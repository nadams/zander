namespace Zander.Presentation.WPF.Zander.Infrastructure.Events {
	public class Empty {
		private static Empty value;

		public static Empty Value { 
			get { return value; } 
		}

		static Empty() {
			value = new Empty();
		}

		private Empty() { }
	}
}
