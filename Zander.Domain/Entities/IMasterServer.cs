using System.Collections.Generic;

namespace Zander.Domain.Entities {
	public interface IMasterServer {
		long Challenge { get; }
		short ProtocolVersion { get; }
		string Address { get; }
		IEnumerable<Server> Servers { get; }
		MasterServerStatus Status { get; set; }
	}
}
