namespace Zander.Provider.Net.Sockets.IO {
	public class HuffmanWrapper : INetworkCompressor {

		public byte[] Encode(byte[] decodedData) {
			return Huffman.Encode(decodedData);
		}

		public byte[] Decode(byte[] encodedData) {
			return Huffman.Decode(encodedData);
		}
	}
}
