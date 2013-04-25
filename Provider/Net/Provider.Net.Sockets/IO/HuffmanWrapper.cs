using System.Diagnostics.CodeAnalysis;

namespace Zander.Provider.Net.Sockets.IO {

	[ExcludeFromCodeCoverage]
	public class HuffmanWrapper : INetworkCompressor {

		static HuffmanWrapper() {
			Huffman.Compression.Construct();
		}

		public int Encode(byte[] input, byte[] output, int length) {
			return Huffman.Compression.Encode(input, output, length);
		}

		public int Decode(byte[] input, byte[] output, int length) {
			return Huffman.Compression.Decode(input, output, length);
		}
	}
}
