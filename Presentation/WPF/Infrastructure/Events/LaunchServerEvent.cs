using Microsoft.Practices.Prism.Events;

namespace Zander.Presentation.WPF.Zander.Infrastructure.Events {
    public class LaunchServerEvent : CompositePresentationEvent<LaunchServerEventArgs> { } 

    public class LaunchServerEventArgs {
        public string Address { get; set; }
    }
}
