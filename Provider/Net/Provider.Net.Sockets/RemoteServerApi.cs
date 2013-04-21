using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Zander.Domain.Entities;
using Zander.Domain.Remote;
using Zander.Provider.Net.Sockets.IO;
using System.Linq;

namespace Zander.Provider.Net.Sockets {
	public class RemoteServerApi : IRemoteServerApi {
		private const int BufferSize = 0x10000;

		private readonly int timeout;
		private readonly INetworkCompressor huffman;
		private readonly ISocketProvider socketProvider;

		private IPEndPoint address;

		public RemoteServerApi(INetworkCompressor huffmanEncoding, ISocketProvider socketProvider, string address, int timeout) {
			this.huffman = huffmanEncoding;
			this.timeout = timeout;
			this.socketProvider = socketProvider;

			var parts = address.Split(':');

			this.address = new IPEndPoint(IPAddress.Parse(parts[0]), int.Parse(parts[1]));
		}

		public MasterChallengeResponse ChallengeMasterServer(MasterChallengeRequest request) {
			MasterChallengeResponse response = new MasterChallengeResponse();
			
			var challenge = BitConverter.GetBytes(request.Challenge);
			var protocolVersion = BitConverter.GetBytes(request.ProtocolVersion);

			var sendData = challenge.Concat(protocolVersion).ToArray();

			var receiveData = this.SendAndGetResponse(sendData);
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

			var responseData = this.SendAndGetResponse(requestData);
			using(var stream = new MemoryStream(responseData, false)) {
				var reader = new CustomBinaryReader(stream, Encoding.ASCII);

				response.Status = (ServerChallengeValues)reader.ReadInt32();
				response.CurrentTime = reader.ReadInt32();

				if(response.Status == ServerChallengeValues.BeginningOfData) {
					response.ServerVersion = reader.ReadString();
					response.QueriedFlags = (ServerQueryValues)reader.ReadInt32();
				}
			}

			return response;
		}

		private byte[] SendAndGetResponse(byte[] data) {
			byte[] result = { };

			using(var socket = this.socketProvider.GetSocket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)) {
				socket.ReceiveTimeout = this.timeout;

				var endpoint = new IPEndPoint(IPAddress.Any, 0);
				byte[] encodedData = new byte[BufferSize];

				var encodedLength = this.huffman.Encode(data, encodedData, data.Length);
				socket.SendTo(encodedData, SocketFlags.None, this.address);

				var receiveBuffer = new byte[BufferSize];

				int receiveLength = socket.ReceiveFrom(receiveBuffer, SocketFlags.None, this.address);

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
