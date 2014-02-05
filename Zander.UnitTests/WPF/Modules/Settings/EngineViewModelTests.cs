using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Zander.Domain.Config;
using Zander.Modules.Settings;
using Zander.Modules.Settings.Engines;

namespace Zander.UnitTests.WPF.Modules.Settings {

    [TestClass]
    public class EngineViewModelTests {

        [TestMethod]
        public void BrowseForClientBinary_FileSelected_ValueSet() {
            string expected = "client binary";

            var mockFileBrowser = new Mock<IFileBrowserProvider>();
            mockFileBrowser.Setup(x => x.BrowseForFile()).Returns(expected);

            var settingsProvider = new ZanderConfigProvider { 
                Config = this.GetNewConfig() 
            };

            var vm = new EngineViewModel(settingsProvider, mockFileBrowser.Object);
            vm.BrowseForClientBinary.Execute();

            Assert.AreEqual(expected, vm.PathToClinetBinary);
        }

        [TestMethod]
        public void BrowseForClientBinary_FileNotSelected_ValueNotSet() {
            string expected = null;

            var mockFileBrowser = new Mock<IFileBrowserProvider>();
            mockFileBrowser.Setup(x => x.BrowseForFile()).Returns(expected);

            var settingsProvider = new ZanderConfigProvider {
                Config = this.GetNewConfig()
            };

            var vm = new EngineViewModel(settingsProvider, mockFileBrowser.Object);
            vm.BrowseForClientBinary.Execute();

            Assert.AreEqual(expected, vm.PathToClinetBinary);
        }

        [TestMethod]
        public void BrowseForServerBinary_FileSelected_ValueSet() {
            string expected = "server binary";

            var mockFileBrowser = new Mock<IFileBrowserProvider>();
            mockFileBrowser.Setup(x => x.BrowseForFile()).Returns(expected);

            var settingsProvider = new ZanderConfigProvider {
                Config = this.GetNewConfig()
            };

            var vm = new EngineViewModel(settingsProvider, mockFileBrowser.Object);
            vm.BrowseForServerBinary.Execute();

            Assert.AreEqual(expected, vm.PathToServerBinary);
        }

        [TestMethod]
        public void BrowseForServerBinary_FileNotSelected_ValueNotSet() {
            string expected = null;

            var mockFileBrowser = new Mock<IFileBrowserProvider>();
            mockFileBrowser.Setup(x => x.BrowseForFile()).Returns(expected);

            var settingsProvider = new ZanderConfigProvider {
                Config = this.GetNewConfig()
            };

            var vm = new EngineViewModel(settingsProvider, mockFileBrowser.Object);
            vm.BrowseForServerBinary.Execute();

            Assert.AreEqual(expected, vm.PathToServerBinary);
        }

        private ZanderConfig GetNewConfig() {
            return new ZanderConfig();
        }
    }
}
