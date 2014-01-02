using System.IO;
using System.Text;
using Newtonsoft.Json;
using Zander.Domain.Config;

namespace Provider.Local.Disk {
    public class ZanderConfigRepository : IZanderConfigRepository {
        public string ConfigPath {
            get {
                return Path.Combine(System.Environment.CurrentDirectory, ZanderConfig.FileName);
            }
        }

        public ZanderConfig GetDefaultConfig() {
            ZanderConfig config = null;
            var configPath = this.ConfigPath;

            if(File.Exists(configPath)) {
                var json = File.ReadAllText(configPath, Encoding.UTF8);
                config = JsonConvert.DeserializeObject<ZanderConfig>(json);
            }

            return config;
        }

        public void SaveConfig(ZanderConfig config) {
            var configPath = this.ConfigPath;
            var settings = new JsonSerializerSettings {
                Formatting = Formatting.Indented
            };

            var json = JsonConvert.SerializeObject(config, settings);
            File.WriteAllText(configPath, json, Encoding.UTF8);
        }
    }
}
