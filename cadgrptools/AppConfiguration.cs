using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cadgrptools
{
    public class AppConfiguration : IAppConfiguration
    {
        public string GetAppSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings[key] ?? throw new Exception($"Key '{key}' not found in appSettings.");
            }
            catch (ConfigurationErrorsException)
            {
                throw new Exception("Error reading appSettings. Check your configuration.");
            }
        }

        public string GetConnectionString(string name)
        {
            try
            {
                var connectionStrings = ConfigurationManager.ConnectionStrings;
                var connectionString = connectionStrings[name];
                return connectionString?.ConnectionString ?? throw new Exception($"Connection string with name '{name}' not found.");
            }
            catch (ConfigurationErrorsException)
            {
                throw new Exception("Error reading connectionStrings. Check your configuration.");
            }
        }
    }
}
