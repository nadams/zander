using System.Net;
using Microsoft.Practices.Prism.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Zander.Modules.ServerBrowser;
using Zander.Modules.ServerBrowser.Models;
using Zander.Presentation.WPF.Zander.Infrastructure.Events;
using Zander.Presentation.WPF.Zander.Services.ServerBrowser;

namespace Zander.UnitTests.WPF.Modules.ServerBrowser {

    [TestClass]
    public class ServerBrowserViewModelTests {

        [TestMethod]
        public void LaunchSelectedServer_ServerIsSelected_EventIsFired() {
            bool eventRaised = false;
            var eventAggregatorMock = this.GetDefaultEventAggregator();
            eventAggregatorMock.Setup(x => x.GetEvent<LaunchServerEvent>()).Returns(new LaunchServerEvent()).Callback(() => eventRaised = true);

            var serverBrowserServiceMock = new Mock<IServerBrowserService>();

            var vm = new ServerBrowserViewModel(eventAggregatorMock.Object, serverBrowserServiceMock.Object) {
                Model = new ServerBrowserModel {
                    SelectedServer = new ServerModel {
                        Address = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10666),
                    }
                }
            };

            vm.LaunchSelectedServer.Execute(null);

            Assert.IsTrue(eventRaised);
        }

        private Mock<IEventAggregator> GetDefaultEventAggregator() {
            var eventAggregatorMock = new Mock<IEventAggregator>();
            eventAggregatorMock.Setup(x => x.GetEvent<QueryAllServersEvent>()).Returns(new QueryAllServersEvent());
            eventAggregatorMock.Setup(x => x.GetEvent<RefreshCurrentServerEvent>()).Returns(new RefreshCurrentServerEvent());
            eventAggregatorMock.Setup(x => x.GetEvent<ServerQueriedEvent>()).Returns(new ServerQueriedEvent());

            return eventAggregatorMock;
        }
    }
}
