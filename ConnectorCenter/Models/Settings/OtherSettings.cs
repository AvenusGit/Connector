using ConnectorCore.Models;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace ConnectorCenter.Models.Settings
{
    public class OtherSettings : UnitedSettings, ISettingsConfiguration
    {
        [JsonIgnore]
        [XmlIgnoreAttribute]
        public string ConfigurationPath
        {
            get
            {
                return ConnectorCenterApp.Instance.ConfigurationFolderPath + @"\OtherSettings.config";
            }
        }
        public new static OtherSettings GetDefault()
        {
            return new OtherSettings()
            {
                DoItGood = false
            };
        }
    }
}
