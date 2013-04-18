namespace Zander.Domain.Entities {
	public class Player {
		public string Name { get; set; }
		public int PointCount { get; set; }
		public int Ping { get; set; }
		public int TimeOnServer { get; set; }
		public bool IsSpectating { get; set; }
		public bool IsBot { get; set; }
		public Team Team { get; set; }
	}
}
