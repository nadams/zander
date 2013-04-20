using System;

namespace Zander.Domain.Exceptions {

	[Serializable]
	public class ClientBannedException : Exception {
		public ClientBannedException() : base() { }
	}
}
