using Microsoft.Practices.Prism.ViewModel;

namespace Zander.Modules.MenuBar.Models {
	public class MenuBarStrings : NotificationObject {

		private string file;
		public string File {
			get {
				return this.file;
			}

			set {
				this.file = value;
				this.RaisePropertyChanged(() => this.File);
			}
		}

		private string quit;
		public string Quit {
			get {
				return this.quit;
			}

			set {
				this.quit = value;
				this.RaisePropertyChanged(() => this.Quit);
			}
		}

		public MenuBarStrings() {
			this.File = "File";
			this.Quit = "Quit";
		}
	}
}
