using System.IO;

namespace Zander.Provider.Net.Sockets {
	public interface ISocketApi {
		Stream GetWriteStream(string address);
		Stream GetReadStream(Stream s);
	}
}
