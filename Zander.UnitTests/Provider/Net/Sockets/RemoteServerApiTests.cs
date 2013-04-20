using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zander.Provider.Net.Sockets;

namespace Zander.UnitTests.Provider.Net.Sockets {

	[TestClass]
	public class RemoteServerApiTests {

		[TestMethod]
		public void ChallengeMasterServer_() {

		}

		private class EmptyNetworkCompressor : INetworkCompressor {
			public byte[] Encode(byte[] decodedData) {
				return decodedData;
			}

			public byte[] Decode(byte[] encodedData) {
				return encodedData;
			}
		}
	}
}
