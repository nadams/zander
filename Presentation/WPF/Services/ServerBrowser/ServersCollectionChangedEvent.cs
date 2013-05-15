using Zander.Domain.Entities;

namespace Zander.Presentation.WPF.Zander.Services.ServerBrowser {
    public delegate void ServersCollectionChangedEventHandler(object sender, ServersCollectionChangedEventArgs args);

    public class ServersCollectionChangedEventArgs {
        private readonly ServersCollectionChangedActions action;
        private readonly Server changedValue;

        public ServersCollectionChangedActions Action {
            get {
                return this.action;
            }
        }

        public Server ChangedValue {
            get {
                return this.changedValue;
            }
        }

        public ServersCollectionChangedEventArgs(ServersCollectionChangedActions action, Server changedValue) {
            this.action = action;
            this.changedValue = changedValue;
        }
    }

    public enum ServersCollectionChangedActions {
        Add,
        Remove,
        Update
    }
}
