namespace Zander.Domain.Config {
    public class ZanderConfig {
        public const string FileName = "zander.config";

        public string ZandronumMasterAddress { get; set; }
        public double WindowWidth { get; set; }
        public double WindowHeight { get; set; }
        public int WindowPositionX { get; set; }
        public int WindowPositionY { get; set; }

        public ZanderConfig() {
            this.ZandronumMasterAddress = "master.zandronum.com:15300";
        }
    }
}
