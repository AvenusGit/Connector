using ConnectorCore.Models;
using System.Xml.Serialization;

namespace ConnectorCenter.Models.Settings
{
    public class ApiSettings : ISettingsConfiguration
    {
        [XmlIgnoreAttribute]
        public string ConfigurationPath
        {
            get
            {
                return ConnectorCenterApp.Instance.ConfigurationFolderPath + @"\ApiSettings.config";
            }
        }
        public bool ApiEnabled { get; set; }
        public bool StatisticApiEnabled { get; set; }
        public bool AuthorizeApiEnabled { get; set; }
        public static ApiSettings GetDefault()
        {
            return new ApiSettings()
            {
                ApiEnabled = true,
                StatisticApiEnabled = true,
                AuthorizeApiEnabled = true
            };
        }
    }
}
