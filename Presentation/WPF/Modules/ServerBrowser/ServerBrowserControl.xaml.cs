using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Zander.Modules.ServerBrowser.Models;

namespace Zander.Modules.ServerBrowser {
	public partial class ServerBrowserControl : UserControl, IServerBrowserView {
		public IServerBrowserViewModel ViewModel {
			get {
				return (IServerBrowserViewModel)this.DataContext;
			}

			set {
				this.DataContext = value;
			}
		}

		public ServerBrowserControl() {
			InitializeComponent();
			this.ViewModel = new ServerBrowserViewModel();
		}

		public ServerBrowserControl(IServerBrowserViewModel viewModel) : this() {
			this.ViewModel = viewModel;
			this.ViewModel.Model.Servers.Add(new ServerModel {
				DisplayName = "test server",
				CurrentMap = "map01",
				GameName = "CTF",
				IWad = "doom2.wad",
				MaxClients = 16,
				MaxPlayers = 12,
				PWads = new List<string> { 
					"zdctfmp.wad",
					"zdctfmp2.wad",
					"zdctfmp3-.wad"
				}
			});
		}
	}
}
