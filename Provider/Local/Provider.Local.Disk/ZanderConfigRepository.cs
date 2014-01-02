using System.IO;
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
            ZanderConfig config;
            var configPath = this.ConfigPath;

            if(File.Exists(configPath)) {
                using(var stream = File.OpenRead(configPath)) {
                    var reader = new JsonTextReader(new StreamReader(stream));
                    var settings = new JsonSerializerSettings {
                        ObjectCreationHandling = ObjectCreationHandling.Replace,
                        DefaultValueHandling = DefaultValueHandling.Include
                    };

                    config = JsonSerializer.Create().Deserialize<ZanderConfig>(reader);
                }
            } else {
                config = new ZanderConfig();
                this.SaveConfig(config);
            }

            return config;
        }

        public void SaveConfig(ZanderConfig config) {
            var configPath = this.ConfigPath;

            if(!File.Exists(configPath)) {
                File.Create(configPath);
            }

            using(var stream = File.Open(configPath, FileMode.Truncate)) {
                var writer = new JsonTextWriter(new StreamWriter(stream));
                var settings = new JsonSerializerSettings {
                    Formatting = Formatting.Indented
                };

                JsonSerializer.Create(settings).Serialize(writer, config);
            }
        }
    }
}
