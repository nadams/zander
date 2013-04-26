using Zander.Presentation.WPF.Zander.Infrastructure.Base;

namespace Zander.Modules.MenuBar.Models {
	public interface IMenuBarViewModel : IViewModel {
		MenuBarStrings Strings { get; set; }
	}
}
