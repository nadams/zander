using Microsoft.Practices.Prism.ViewModel;

namespace Zander.Modules.ServerBrowser.Models {
	public class PlayerModel : NotificationObject {

        private string name;
		public string Name {
            get {
                return this.name;
            }

            set {
                this.name = value;
                this.RaisePropertyChanged(() => this.Name);
            }
        }

        private int pointCount;
		public int PointCount {
            get {
                return this.pointCount;
            }

            set {
                this.pointCount = value;
                this.RaisePropertyChanged(() => this.PointCount);
            }
        }

        private int ping;
		public int Ping {
            get {
                return this.ping;
            }

            set {
                this.ping = value;
                this.RaisePropertyChanged(() => this.Ping);
            }
        }

        private int timeOnServer;
		public int TimeOnServer {
            get {
                return this.timeOnServer;
            }

            set {
                this.timeOnServer = value;
                this.RaisePropertyChanged(() => this.TimeOnServer);
            }
        }

        private bool isSpectating;
		public bool IsSpectating {
            get {
                return this.isSpectating;
            }

            set {
                this.isSpectating = value;
                this.RaisePropertyChanged(() => this.IsSpectating);
            }
        }

        private bool isBot;
		public bool IsBot {
            get {
                return this.isBot;
            }

            set {
                this.isBot = value;
                this.RaisePropertyChanged(() => this.IsBot);
            }
        }
	}
}
