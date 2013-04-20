namespace Zander.Provider.Net.Sockets.IO {
	public interface IHuffmanEncoding {
		byte[] Encode(byte[] decodedData);
		byte[] Decode(byte[] encodedData);
	}
}
