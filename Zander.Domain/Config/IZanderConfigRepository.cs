namespace Zander.Domain.Config {
    public interface IZanderConfigRepository {
        ZanderConfig GetDefaultConfig();
        void SaveConfig(ZanderConfig config);
    }
}
