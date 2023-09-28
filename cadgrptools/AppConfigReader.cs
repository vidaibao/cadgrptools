using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace cadgrptools
{
    public class AppConfigReader
    {
        
        private readonly XDocument _configDocument;

        public AppConfigReader(string configFilePath)
        {
            try
            {
                _configDocument = XDocument.Load(configFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading the configuration file: {ex.Message}");
            }
        }

        public string ReadAppSetting(string key)
        {
            try
            {
                var appSettingElement = _configDocument
                    .Element("configuration")
                    .Element("appSettings")
                    .Elements("add")
                    .FirstOrDefault(e => e.Attribute("key")?.Value == key);

                if (appSettingElement != null)
                {
                    return appSettingElement.Attribute("value")?.Value;
                }
                else
                {
                    throw new Exception($"Key '{key}' not found in appSettings.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading appSettings: {ex.Message}");
            }
        }


        public string ReadConnectionString(string name)
        {
            try
            {
                var connectionStringElement = _configDocument
                    .Element("configuration")
                    .Element("connectionStrings")
                    .Elements("add")
                    .FirstOrDefault(e => e.Attribute("name")?.Value == name);

                if (connectionStringElement != null)
                {
                    return connectionStringElement.Attribute("connectionString")?.Value;
                }
                else
                {
                    throw new Exception($"Connection string with name '{name}' not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading connectionStrings: {ex.Message}");
            }
        }
        


        /*
        public static string ReadAppSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings[key] ?? throw new Exception($"Key '{key}' not found in appSettings.");
            }
            catch (ConfigurationErrorsException)
            {
                throw new Exception("Error reading appSettings. Check your app.config file.");
            }
        }

        public static string ReadConnectionString(string name)
        {
            try
            {
                var connectionStrings = ConfigurationManager.ConnectionStrings;
                var connectionString = connectionStrings[name];
                return connectionString?.ConnectionString ?? throw new Exception($"Connection string with name '{name}' not found.");
            }
            catch (ConfigurationErrorsException)
            {
                throw new Exception("Error reading connectionStrings. Check your app.config file.");
            }
        }
        */
    }
}
