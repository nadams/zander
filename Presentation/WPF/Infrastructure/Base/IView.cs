namespace Zander.Presentation.WPF.Zander.Infrastructure.Base {
	public interface IView<T> where T : IViewModel {
		T ViewModel { get; set; }
	}
}
