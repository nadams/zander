using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Zander.Domain.Remote;
using Zander.Provider.Net.Sockets.IO;

namespace Zander.Provider.Net.Sockets {
	public class RemoteServerApi : IRemoteServerApi {
		private delegate void LoadServerInfo(ServerResponse response, ServerQueryValues flags, BinaryReader reader);

		private const int BufferSize = 0x10000;

		private readonly INetworkCompressor huffman;
		private readonly ISocketProvider socketProvider;

		public RemoteServerApi(INetworkCompressor huffmanEncoding, ISocketProvider socketProvider) {
			this.huffman = huffmanEncoding;
			this.socketProvider = socketProvider;
		}

		public MasterChallengeResponse ChallengeMasterServer(MasterChallengeRequest request) {
			MasterChallengeResponse response = new MasterChallengeResponse();
			
			var challenge = BitConverter.GetBytes(request.Challenge);
			var protocolVersion = BitConverter.GetBytes(request.ProtocolVersion);

			var sendData = challenge.Concat(protocolVersion).ToArray();

			var receiveData = this.SendAndGetResponse(sendData, request.Endpoint, request.Timeout);
			using(var reader = new BinaryReader(new MemoryStream(receiveData, false))) {
				var status = reader.ReadInt32();

				response.Status = (MasterChallengeValues)status;

				if(response.Status == MasterChallengeValues.BeginningOfServerList) {
					response.PacketNumber = reader.ReadByte();
					response.ServerBlock = (MasterChallengeValues)reader.ReadInt32();

					if(response.ServerBlock == MasterChallengeValues.ServerBlock) {
						var servers = new List<ServerListResponse>();

						do {
							this.ReadServers(reader, servers);
						} while(((MasterChallengeValues)reader.ReadByte()) == MasterChallengeValues.EndOfCurrentList);

						response.Servers = servers;
					}
				}
			}

			return response;
		}

		private void ReadServers(BinaryReader reader, List<ServerListResponse> servers) {
			byte numberOfServers;

			while((numberOfServers = reader.ReadByte()) > 0) {
				byte octet1 = reader.ReadByte();
				byte octet2 = reader.ReadByte();
				byte octet3 = reader.ReadByte();
				byte octet4 = reader.ReadByte();
				ushort port = reader.ReadUInt16();

				var response = new ServerListResponse(octet1, octet2, octet3, octet4, port);

				servers.Add(response);
			}
		}

		public ServerResponse GetServerInfo(ServerRequest request) {
			ServerResponse response = new ServerResponse();

			var requestData = BitConverter.GetBytes(request.Challenge).
				Concat(BitConverter.GetBytes(request.Query)).
				Concat(BitConverter.GetBytes(request.TickCount)).
				ToArray();

			var responseData = this.SendAndGetResponse(requestData, request.Endpoint, request.Timeout);
			using(var stream = new MemoryStream(responseData, false)) {
				var reader = new CustomBinaryReader(stream, Encoding.ASCII);

				response.Status = (ServerChallengeValues)reader.ReadInt32();
				response.CurrentTime = reader.ReadInt32();

				if(response.Status == ServerChallengeValues.BeginningOfData) {
					response.ServerVersion = reader.ReadString();
					response.QueriedFlags = (ServerQueryValues)reader.ReadInt32();

					Func<string> readString = () => reader.ReadString();
					Func<int> readInt = () => reader.ReadInt32();
					Func<byte> readByte = () => reader.ReadByte();
					Func<short> readShort = () => reader.ReadInt16();
					Func<float> readFloat = () => reader.ReadSingle();
					Func<bool> readBool = () => reader.ReadByte() == 0 ? false : true;
					Func<ushort> readUShort = () => reader.ReadUInt16();

					var flags = response.QueriedFlags;
					if(flags.HasFlag(ServerQueryValues.Name)) {
						response.Name = readString();
					}

					if(flags.HasFlag(ServerQueryValues.Url)) {
						response.Url = readString();
					}

					if(flags.HasFlag(ServerQueryValues.Email)) {
						response.Email = readString();
					}

					if(flags.HasFlag(ServerQueryValues.MapName)) {
						response.MapName = readString();
					}

					if(flags.HasFlag(ServerQueryValues.MaxClients)) {
						response.MaxClients = readByte();
					}

					if(flags.HasFlag(ServerQueryValues.MaxPlayers)) {
						response.MaxPlayers = readByte();
					}

					if(flags.HasFlag(ServerQueryValues.PWads)) {
						response.PWadsLoaded = readByte();

						var pwads = new List<string>();
						for(int i = 0; i < response.PWadsLoaded; i++) {
							pwads.Add(readString());
						}

						response.PWads = pwads;
					}

					if(flags.HasFlag(ServerQueryValues.GameType)) {
						response.GameType = readByte();
						response.IsInstagib = readBool();
						response.IsBuckshot = readBool();
					}

					if(flags.HasFlag(ServerQueryValues.GameName)) {
						response.GameName = readString();
					}

					if(flags.HasFlag(ServerQueryValues.IWad)) {
						response.IWad = readString();
					}

					if(flags.HasFlag(ServerQueryValues.ForcePassword)) {
						response.HasPassword = readBool();
					}

					if(flags.HasFlag(ServerQueryValues.ForceJoinPassword)) {
						response.HasJoinPassword = readBool();
					}

					if(flags.HasFlag(ServerQueryValues.GameSkill)) {
						response.GameSkill = readByte();
					}

					if(flags.HasFlag(ServerQueryValues.BotSkill)) {
						response.BotSkill = readByte();
					}

					if(flags.HasFlag(ServerQueryValues.Limits)) {
						response.FragLimit = readShort();
						response.TimeLimit = readShort();
						response.TimeLeft = readShort();
						response.DuelLimit = readShort();
						response.PointLimit = readShort();
						response.WinLimit = readShort();
					}

					if(flags.HasFlag(ServerQueryValues.TeamDamage)) {
						response.TeamDamage = readFloat();
					}

					if(flags.HasFlag(ServerQueryValues.NumberOfPlayers)) {
						response.NumberOfPlayers = readByte();

						if(flags.HasFlag(ServerQueryValues.PlayerData)) {
							var players = new List<PlayerDataResponse>();

							for(int i = 0; i < response.NumberOfPlayers; i++) {
								var player = new PlayerDataResponse { 
									Name = readString(),
									PointCount = readShort(),
									Ping = readUShort(),
									IsSpectating = readBool(),
									IsBot = readBool(),
									TeamId = readByte(),
									TimeOnServer = readByte(),
								};

								players.Add(player);
							}

							response.PlayerData = players;
						}
					}

					if(flags.HasFlag(ServerQueryValues.TeamInfoNumber)) {
						response.NumberOfTeams = readByte();
						var teams = Enumerable.Repeat(new TeamInfoResponse(), response.NumberOfTeams).ToList();

						if(flags.HasFlag(ServerQueryValues.TeamInfoName)) {
							for(int i = 0; i < response.NumberOfTeams; i++) {
								teams[i].Name = readString();
							}
						}

						if(flags.HasFlag(ServerQueryValues.TeamInfoColor)) {
							for(int i = 0; i < response.NumberOfTeams; i++) {
								teams[i].Color = readInt();
							}
						}

						if(flags.HasFlag(ServerQueryValues.TeamInfoScore)) {
							for(int i = 0; i < response.NumberOfTeams; i++) {
								teams[i].Score = readShort();
							}
						}

						response.Teams = teams;
					}

					if(flags.HasFlag(ServerQueryValues.TestingServer)) {
						response.IsTestingServer = readBool();
						response.TestingBinaryUrl = readString();
					}

					if(flags.HasFlag(ServerQueryValues.DataChecksum)) {
						response.Checksum = readString();
					}

					if(flags.HasFlag(ServerQueryValues.AllDmflags)) {
						response.NumberOfFlags = readByte();

						response.DMFlags = readInt();
						response.DMFlags2 = readInt();
						response.DMFlags3 = readInt();
						response.CompatFlags = readInt();
						response.CompatFlags2 = readInt();
					}

					if(flags.HasFlag(ServerQueryValues.SecuritySettings)) {
						response.UsesSecuritySettings = readBool();
					}
				}
			}

			return response;
		}

		private byte[] SendAndGetResponse(byte[] data, IPEndPoint address, int timeout) {
			byte[] result = { };

			using(var socket = this.socketProvider.GetSocket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)) {
				socket.ReceiveTimeout = timeout;

				var endpoint = new IPEndPoint(IPAddress.Any, 0);
				byte[] encodedData = new byte[BufferSize];

				var encodedLength = this.huffman.Encode(data, encodedData, data.Length);
				socket.SendTo(encodedData, SocketFlags.None, address);

				var receiveBuffer = new byte[BufferSize];

				int receiveLength = socket.ReceiveFrom(receiveBuffer, SocketFlags.None, address);

				receiveBuffer = receiveBuffer.Take(receiveLength).ToArray();
				var decodedData = new byte[BufferSize];
				var decodedLength = this.huffman.Decode(receiveBuffer, decodedData, receiveLength);
				decodedData = decodedData.Take(decodedLength).ToArray();

				result = decodedData;
			}

			return result;
		}
	}
}
