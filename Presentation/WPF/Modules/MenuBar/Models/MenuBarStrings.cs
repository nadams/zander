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

		private string refreshCurrentServer;
		public string RefreshCurrentServer {
			get {
				return this.refreshCurrentServer;
			}

			set {
				this.refreshCurrentServer = value;
				this.RaisePropertyChanged(() => this.RefreshCurrentServer);
			}
		}

		private string refreshAllServers;
		public string RefreshAllServers {
			get {
				return this.refreshAllServers;
			}

			set {
				this.refreshAllServers = value;
				this.RaisePropertyChanged(() => this.RefreshAllServers);
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

		private string about;
		public string About {
			get {
				return this.about;
			}

			set {
				this.about = value;
				this.RaisePropertyChanged(() => this.About);
			}
		}

		private string settings;
		public string Settings {
			get {
				return this.settings;
			}

			set {
				this.settings = value;
				this.RaisePropertyChanged(() => this.Settings);
			}
		}

		private string editFilters;
		public string EditFilters {
			get {
				return this.editFilters;
			}

			set {
				this.editFilters = value;
				this.RaisePropertyChanged(() => this.EditFilters);
			}
		}

		#endregion

		public MenuBarStrings() {
			this.File = "File";
			this.Quit = "Quit";
			this.Edit = "Edit";
			this.About = "About";
			this.Server = "Server";
			this.Filter = "Filter";
			this.Help = "Help";
			this.RefreshCurrentServer = "Refresh current server";
			this.RefreshAllServers = "Refresh all servers";
			this.AddToIgnoreList = "Add server to ignore list";
			this.EditCustomServers = "Custom servers";
			this.EditIgnoreList = "Ignore list";
			this.EditFilters = "Filters";
			this.Settings = "Settings";
		}
	}
}
