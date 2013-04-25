using System;

namespace Zander.Domain.Exceptions {

	[Serializable]
	public class ClientIgnoredException : Exception {
		public ClientIgnoredException() : base() { }
	}
}
