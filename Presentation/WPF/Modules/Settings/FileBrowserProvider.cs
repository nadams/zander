using Microsoft.Win32;

namespace Zander.Modules.Settings {
    public class FileBrowserProvider : IFileBrowserProvider {
        private readonly OpenFileDialog fileBrowser;

        public FileBrowserProvider() {
            this.fileBrowser = new OpenFileDialog();
        }

        public string BrowseForFile() {
            bool? result = this.fileBrowser.ShowDialog();
            string filePath = null;

            if(result != null && result.Value) {
                filePath = this.fileBrowser.FileName;
            }

            return filePath;
        }
    }
}
