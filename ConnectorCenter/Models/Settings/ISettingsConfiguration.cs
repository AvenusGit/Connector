namespace ConnectorCenter.Models.Settings
{
    public interface ISettingsConfiguration
    {
        public string ConfigurationPath
        {
            get;
        }
        public ISettingsConfiguration GetDefault();
    }
}
