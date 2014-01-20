using Newtonsoft.Json;

namespace Zander.Domain.Config {
    public class ZanderConfigService : IZanderConfigService {
        private readonly IZanderConfigRepository repository;
        private readonly object lockObject;

        private ZanderConfig configInstance;

        public ZanderConfigService(IZanderConfigRepository repository) {
            this.repository = repository;
            this.lockObject = new object();
        }

        public ZanderConfig GetDefaultConfig() {
            if(this.configInstance == null) {
                lock(this.lockObject) {
                    var config = this.repository.GetDefaultConfig();
                    if(config == null) {
                        config = new ZanderConfig();
                        this.repository.SaveConfig(config);
                    }

                    this.configInstance = config;
                }
            }

            return this.configInstance;
        }

        public void SaveConfig(ZanderConfig config) {
            this.repository.SaveConfig(config);
            lock(this.lockObject) {
                this.configInstance = config;
            }
        }

        public ZanderConfig CloneConfig(ZanderConfig config) {
            return JsonConvert.DeserializeObject<ZanderConfig>(JsonConvert.SerializeObject(config));
        }
    }
}
