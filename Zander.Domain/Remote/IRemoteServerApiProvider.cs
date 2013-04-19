namespace Zander.Domain.Remote {
	public interface IRemoteServerApiProvider {
		IRemoteServerApi GetInstance(string address, int timeout);
	}
}
