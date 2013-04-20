using System;

namespace Zander.Domain.Exceptions {

	[Serializable]
	public class UnknownMasterServerResponseException : Exception {
		public UnknownMasterServerResponseException() : base() { }
	}
}
