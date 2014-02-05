namespace Zander.Modules.Settings.Engines.Zandronum {
    public class ZandronumEngineViewModel : EngineViewModel, IZandronumEngineViewModel {
        public ZandronumEngineViewModel(ZanderConfigProvider configProvider, IFileBrowserProvider fileBrowser) : base(configProvider, fileBrowser) { }
    }
}
