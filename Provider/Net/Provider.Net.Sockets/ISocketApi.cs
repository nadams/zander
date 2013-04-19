using System.IO;
namespace Zander.Provider.Net.Sockets {
	public interface ISocketApi {
		Stream Get(string address);
	}
}
