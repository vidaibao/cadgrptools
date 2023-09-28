using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cadgrptools
{
    public interface IAppConfiguration
    {
        string GetAppSetting(string key);
        string GetConnectionString(string name);
    }
}
