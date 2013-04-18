namespace Zander.Domain.Entities {
	public enum MasterServerStatus {
		Available = 0,
		Banned,
		TooManyRequests,
		WrongVersion,
	}
}
