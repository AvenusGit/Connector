using System.Xml.Serialization;

namespace ConnectorCenter.Models.Settings
{
    public class OtherSettings : ISettingsConfiguration
    {
        [XmlIgnoreAttribute]
        public string ConfigurationPath
        {
            get
            {
                return ConnectorCenterApp.Instance.ConfigurationFolderPath + @"\OtherSettings.config";
            }
        }
        public bool DoItGood { get; set; }
        // some settings
        public static OtherSettings GetDefault()
        {
            return new OtherSettings()
            {
                DoItGood = false
            };
        }
    }
}
