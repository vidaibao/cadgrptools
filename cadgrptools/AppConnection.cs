using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace cadgrptools
{
    public static class AppConnection
    {




        /*
        private static readonly IAppConfiguration _appConfiguration;

        public AppConnection(IAppConfiguration appConfiguration)
        {
            _appConfiguration = appConfiguration;
        }

        static string getFilePath()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\cadgrptools.dll.config";

            XDocument doc = XDocument.Load(path);

            var query = doc.Descendants("appSettings").Nodes().Cast<XElement>().Where(x => x.Attribute("key").Value.ToString() == key).FirstOrDefault();

            if (query != null)
            {
                return query.Attribute("value").Value.ToString();
            }
            return null;
        }
        */

        /*
         * 
         * 
         * Config files do not get compiled into the dll.

The app will use the app.config from the startup project of your solution unless you put in special code to look elsewhere.

This is by design so that you can change configuration with out having to recompile just because a setting changed.



        public static string sqlserver_cs => ConfigurationManager.ConnectionStrings["sqlserver_cs"].ConnectionString;
        
        static string cadgrpPath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            ConfigurationManager.ConnectionStrings["sqlite_cs"].ConnectionString
            );
        public static string sqlite_cadgrp_cs => $"Data Source={cadgrpPath}";
        
        public static string optionMul = ConfigurationManager.AppSettings["option"];

        public static string ltypeName = ConfigurationManager.AppSettings["ltypehidden"];
        public static string desc = ConfigurationManager.AppSettings["comment"];

        */






    }
}
