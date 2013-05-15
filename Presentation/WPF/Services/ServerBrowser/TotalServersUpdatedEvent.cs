namespace Zander.Presentation.WPF.Zander.Services.ServerBrowser {
    public delegate void TotalServersUpdatedEventHandler(object sender, TotalServersUpdatedEventArgs args);

    public class TotalServersUpdatedEventArgs {
        private readonly int totalServers;

        public int TotalServers {
            get {
                return this.totalServers;
            }
        }

        public TotalServersUpdatedEventArgs(int totalServers) {
            this.totalServers = totalServers;
        }
    }
}
