using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zander.Domain.Config {
    public interface IZanderConfigService {
        ZanderConfig GetDefaultConfig();
        void SaveConfig(ZanderConfig config);
    }
}
