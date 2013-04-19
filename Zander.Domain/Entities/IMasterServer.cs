using System.Collections.Generic;

namespace Zander.Domain.Entities {
	public interface IMasterServer {
		string Address { get; }
		IEnumerable<Server> Servers { get; }
		MasterServerStatus Status { get; set; }
	}
}
