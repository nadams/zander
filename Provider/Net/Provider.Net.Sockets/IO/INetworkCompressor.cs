namespace Zander.Provider.Net.Sockets {
	public interface INetworkCompressor {
		int Encode(byte[] input, byte[] output, int length);
		int Decode(byte[] input, byte[] output, int length);
	}
}
