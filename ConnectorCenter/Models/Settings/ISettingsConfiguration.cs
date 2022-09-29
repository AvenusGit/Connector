using System.Xml.Serialization;

namespace ConnectorCenter.Models.Settings
{
    public interface ISettingsConfiguration
    {
        [XmlIgnoreAttribute]
        public string ConfigurationPath
        {
            get;
        }
    }
}
