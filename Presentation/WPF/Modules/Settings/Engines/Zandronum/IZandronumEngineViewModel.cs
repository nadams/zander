using Microsoft.Practices.Prism.Commands;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;

namespace Zander.Modules.Settings.Engines.Zandronum {
    public interface IZandronumEngineViewModel : IViewModel {
        string PathToClinetBinary { get; set; }
        string PathToServerBinary { get; set; }
        string CustomParameters { get; set; }
        string MasterAddress { get; set; }

        DelegateCommand BrowseForClientBinary { get; }
        DelegateCommand BrowseForServerBinary { get; }
    }
}
