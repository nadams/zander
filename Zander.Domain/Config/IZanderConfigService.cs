namespace Zander.Domain.Config {
    public interface IZanderConfigService {
        ZanderConfig GetDefaultConfig();
        ZanderConfig CloneConfig(ZanderConfig config);
        void SaveConfig(ZanderConfig config);
    }
}
