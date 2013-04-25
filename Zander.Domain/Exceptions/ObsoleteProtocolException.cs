using System;

namespace Zander.Domain.Exceptions {

	[Serializable]
	public class ObsoleteProtocolException : Exception {
		public ObsoleteProtocolException() : base() { }
	}
}
