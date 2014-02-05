namespace Zander.Domain.Config {
    public interface IZanderConfigService {
        event ConfigUpdatedEventHandler ConfigUpdated;

        ZanderConfig GetDefaultConfig();
        ZanderConfig CloneConfig(ZanderConfig config);
        void SaveConfig(ZanderConfig config);
    }
}
