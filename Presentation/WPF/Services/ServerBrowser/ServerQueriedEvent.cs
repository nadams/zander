using Zander.Domain.Entities;

namespace Zander.Presentation.WPF.Zander.Services.ServerBrowser {
    public delegate void ServerQueriedEventHandler(object sender, ServerQueriedEventArgs args);

    public class ServerQueriedEventArgs {
        private readonly Server server;

        public Server Server {
            get {
                return this.server;
            }
        }

        public ServerQueriedEventArgs(Server server) {
            this.server = server;
        }
    }
}
