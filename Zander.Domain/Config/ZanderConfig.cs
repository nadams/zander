namespace Zander.Domain.Config {
    public class ZanderConfig {
        public const string FileName = "zander.config";

        public ZandronumEngineConfig ZandronumConfig { get; set; }
        public WindowSettings WindowSettings { get; set; }

        public ZanderConfig() {
            this.WindowSettings = new WindowSettings();
            this.ZandronumConfig = new ZandronumEngineConfig();
        }
    }

    public class WindowSettings {
        public double WindowWidth { get; set; }
        public double WindowHeight { get; set; }
        public int WindowPositionX { get; set; }
        public int WindowPositionY { get; set; }

        public WindowSettings() {
            this.WindowWidth = 800;
            this.WindowHeight = 600;
        }
    }

    public abstract class EngineConfig {
        public string MasterAddress { get; set; }
        public string PathToClinetBinary { get; set; }
        public string PathToServerBinary { get; set; }
        public string CustomParameters { get; set; }
    }

    public class ZandronumEngineConfig : EngineConfig {
        public ZandronumEngineConfig() {
            this.MasterAddress = "master.zandronum.com:15300";
        }
    }
}
