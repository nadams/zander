namespace Zander.Presentation.WPF.Zander.Infrastructure.Base {
    public interface IWindow<T> where T : IViewModel {
        T ViewModel { get; set; }

        void Show();
        bool? ShowDialog();
        void Close();
    }
}
