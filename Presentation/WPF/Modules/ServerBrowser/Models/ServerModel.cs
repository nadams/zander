using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using Microsoft.Practices.Prism.ViewModel;

namespace Zander.Modules.ServerBrowser.Models {
	public class ServerModel : NotificationObject {
		public IPEndPoint Address { get; set; }

        private int maxClients;
		public int MaxClients {
            get {
                return this.maxClients;
            }

            set {
                this.maxClients = value;
                this.RaisePropertyChanged(() => this.MaxClients);
            }
        }

        private int maxPlayers;
		public int MaxPlayers {
            get {
                return this.maxPlayers;
            }

            set {
                this.maxPlayers = value;
                this.RaisePropertyChanged(() => this.MaxPlayers);
                this.RaisePropertyChanged(() => this.Players);
            }
        }

        private string displayName;
		public string DisplayName {
            get {
                return this.displayName;
            }

            set {
                this.displayName = value;
                this.RaisePropertyChanged(() => this.DisplayName);
            }
        }

        private string currentMap;
		public string CurrentMap {
            get {
                return this.currentMap;
            }

            set {
                this.currentMap = value;
                this.RaisePropertyChanged(() => this.CurrentMap);
            }
        }

        private string iwad;
		public string IWad {
            get {
                return this.iwad;
            }

            set {
                this.iwad = value;
                this.RaisePropertyChanged(() => this.IWad);
            }
        }

        private string gameName;
		public string GameName {
            get {
                return this.gameName;
            }

            set {
                this.gameName = value;
                this.RaisePropertyChanged(() => this.GameName);
            }
        }

        private ObservableCollection<string> pwads;
		public ICollection<string> PWads {
            get {
                return this.pwads.ToList();
            }

            set {
                this.pwads.CollectionChanged -= this.RaisePWadsChanged;

                this.pwads.Clear();

                foreach(var item in value) {
                    this.pwads.Add(item);
                }

                this.pwads.CollectionChanged += this.RaisePWadsChanged;
                this.RaisePWadsChanged(null, null);
            }
        }

        private ObservableCollection<PlayerModel> currentPlayers;
		public ICollection<PlayerModel> CurrentPlayers {
            get {
                return this.currentPlayers.ToList();
            }

            set {
                this.currentPlayers.CollectionChanged -= this.RaiseCurrentPlayersChanged;

                this.currentPlayers.Clear();
                foreach(var item in value) {
                    this.currentPlayers.Add(item);
                }

                this.currentPlayers.CollectionChanged += this.RaiseCurrentPlayersChanged;
                this.RaiseCurrentPlayersChanged(null, null);
            }
        }

		public string Players {
			get {
				return this.CurrentPlayers.Count() + " / " + this.MaxPlayers;
			}
		}

		public ServerModel() {
            this.pwads = new ObservableCollection<string>();
            this.pwads.CollectionChanged += this.RaisePWadsChanged;

            this.currentPlayers = new ObservableCollection<PlayerModel>();
            this.currentPlayers.CollectionChanged += this.RaiseCurrentPlayersChanged;
		}

        public void CopyData(ServerModel that) {
            this.MaxClients = that.MaxClients;
            this.MaxPlayers = that.MaxPlayers;
            this.DisplayName = that.DisplayName;
            this.CurrentMap = that.CurrentMap;
            this.CurrentPlayers = that.CurrentPlayers;
            this.DisplayName = that.DisplayName;
            this.GameName = that.GameName;
            this.IWad = that.IWad;
            this.MaxClients = that.MaxClients;
            this.MaxPlayers = that.MaxPlayers;
            this.PWads = that.PWads;
        }

        private void RaisePWadsChanged(object sender, NotifyCollectionChangedEventArgs args) {
            this.RaisePropertyChanged(() => this.PWads);
        }

        private void RaiseCurrentPlayersChanged(object sender, NotifyCollectionChangedEventArgs args) {
            this.RaisePropertyChanged(() => this.CurrentPlayers);
            this.RaisePropertyChanged(() => this.Players);
        }
	}
}
