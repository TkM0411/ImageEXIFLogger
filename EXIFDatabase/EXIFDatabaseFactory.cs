using System;
using System.Configuration;
using System.Linq;

namespace EXIFDatabase
{
    public class EXIFDatabaseFactory
    {
        public EXIFDatabaseController Create(string configKeyName)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(configKeyName.Trim()))
            {
                string localDbFileName = Convert.ToString(ConfigurationManager.AppSettings[configKeyName.Trim()]).Trim();
                return new EXIFDatabaseController(localDbFileName);
            }
            else
                return new EXIFDatabaseController();
        }
    }
}