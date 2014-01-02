using System.ComponentModel;
namespace Zander.Domain.Config {
    public class ZanderConfig {
        public const string FileName = "zander.config";

        [DefaultValue("master.zandronum.com:15300")]
        public string ZandronumMasterAddress { get; set; }
    }
}
