using Microsoft.Practices.Prism.ViewModel;

namespace Zander.Modules.MenuBar.Models {
	public class MenuBarStrings : NotificationObject {

		#region Proerties
		
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

		private string edit;
		public string Edit {
			get {
				return this.edit;
			}

			set {
				this.edit = value;
				this.RaisePropertyChanged(() => this.Edit);
			}
		}

		private string server;
		public string Server {
			get {
				return this.server;
			}

			set {
				this.server = value;
				this.RaisePropertyChanged(() => this.Server);
			}
		}

		private string filter;
		public string Filter {
			get {
				return this.filter;
			}

			set {
				this.filter = value;
				this.RaisePropertyChanged(() => this.Filter);
			}
		}

		private string help;
		public string Help {
			get {
				return this.help;
			}

			set {
				this.help = value;
				this.RaisePropertyChanged(() => this.Help);
			}
		}

		private string refresh;
		public string Refresh {
			get {
				return this.refresh;
			}

			set {
				this.refresh = value;
				this.RaisePropertyChanged(() => this.Refresh);
			}
		}

		private string addToIgnoreList;
		public string AddToIgnoreList {
			get {
				return this.addToIgnoreList;
			}

			set {
				this.addToIgnoreList = value;
				this.RaisePropertyChanged(() => this.AddToIgnoreList);
			}
		}

		private string editCustomServers;
		public string EditCustomServers {
			get {
				return this.editCustomServers;
			}

			set {
				this.editCustomServers = value;
				this.RaisePropertyChanged(() => EditCustomServers);
			}
		}

		private string editIgnoreList;
		public string EditIgnoreList {
			get {
				return this.editIgnoreList;
			}

			set {
				this.editIgnoreList = value;
				this.RaisePropertyChanged(() => this.EditIgnoreList);
			}
		}

		#endregion

		public MenuBarStrings() {
			this.File = "File";
			this.Quit = "Quit";
			this.Edit = "Edit";
			this.Server = "Server";
			this.Filter = "Filter";
			this.Help = "Help";
			this.Refresh = "Refresh";
			this.AddToIgnoreList = "Add server to ignore list";
			this.EditCustomServers = "Edit custom servers";
			this.EditIgnoreList = "Edit ignore list";
		}
	}
}
