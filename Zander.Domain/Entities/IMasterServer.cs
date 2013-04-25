using System.Collections.Generic;
using System.Net;

namespace Zander.Domain.Entities {
	public interface IMasterServer {
		string Address { get; }
		IEnumerable<IPEndPoint> Servers { get; }
	}
}
