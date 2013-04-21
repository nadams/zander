namespace Zander.Domain.Remote {
	public class ServerResponse {
		public ServerChallengeValues Status { get; set; }
		public int CurrentTime { get; set; }

		public string ServerVersion { get; set; }
		public ServerQueryValues QueriedFlags { get; set; }
	}
}
