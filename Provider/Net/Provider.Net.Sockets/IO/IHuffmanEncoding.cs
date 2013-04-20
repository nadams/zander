namespace Zander.Provider.Net.Sockets {
	public interface INetworkCompressor {
		byte[] Encode(byte[] decodedData);
		byte[] Decode(byte[] encodedData);
	}
}
