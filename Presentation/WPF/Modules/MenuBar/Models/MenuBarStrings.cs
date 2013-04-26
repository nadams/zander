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

		public MenuBarStrings() {
			this.File = "File";
		}
	}
}
