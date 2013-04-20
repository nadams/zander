using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Zander.Domain.Entities;
using Zander.Domain.Remote;
using Zander.Provider.Net.Sockets.IO;

namespace Zander.Provider.Net.Sockets {
	public class RemoteServerApi : IRemoteServerApi {
		private const int BufferSize = 0x10000;

		private readonly int timeout;
		private readonly INetworkCompressor huffman;

		private IPEndPoint address;

		public RemoteServerApi(INetworkCompressor huffmanEncoding, string address, int timeout) {
			this.huffman = huffmanEncoding;
			this.timeout = timeout;

			var parts = address.Split(':');

			this.address = new IPEndPoint(IPAddress.Parse(parts[0]), int.Parse(parts[1]));
		}

		public MasterChallengeResponse ChallengeMasterServer(MasterChallengeRequest request) {
			MasterChallengeResponse response = new MasterChallengeResponse();

			using(var outStream = new MemoryStream()) {
				var writer = new BinaryWriter(outStream);
				writer.Write(request.Challenge);
				writer.Write(request.ProtocolVersion);

				using(var inStream = this.SendAndGetResponse(outStream)) {
					var reader = new BinaryReader(inStream, Encoding.Default);

					MasterChallengeValues masterStatus = (MasterChallengeValues)reader.ReadInt32();

					response.Status = masterStatus;
					if(response.Status == MasterChallengeValues.BeginningOfServerList) {
						response.PacketNumber = reader.ReadByte();


					}
				}
			}

			return response;
		}

		public Server GetServerInfo(IPEndPoint serverEndpoint) {
			throw new NotImplementedException();
		}

		public void Dispose() {
			throw new NotImplementedException();
		}

		private MemoryStream SendAndGetResponse(MemoryStream outStream) {
			MemoryStream result = null;

			using(var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)) {
				socket.ReceiveTimeout = this.timeout;

				var data = outStream.GetBuffer();
				var endpoint = new IPEndPoint(IPAddress.Any, 0);

				var encodedData = this.huffman.Encode(data);
				socket.SendTo(encodedData, SocketFlags.None, this.address);

				var receiveEndpoint = (EndPoint)this.address;
				var receiveBuffer = new byte[BufferSize];

				socket.ReceiveFrom(receiveBuffer, SocketFlags.None, ref receiveEndpoint);

				var decodedData = this.huffman.Decode(receiveBuffer);

				var output = new byte[BufferSize];
				Buffer.BlockCopy(decodedData, 0, output, 0, decodedData.Length);

				result = new MemoryStream(output, 0, decodedData.Length, true, true);
			}

			return result;
		}
	}
}
