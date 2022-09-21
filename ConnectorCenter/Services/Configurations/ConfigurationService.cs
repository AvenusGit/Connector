using ConnectorCore.Models;
using ConnectorCenter.Models.Settings;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;
namespace ConnectorCenter.Services.Configurations
{
    public static class SettingsConfigurationService
    {
        public static void SaveConfiguration(ISettingsConfiguration configuration)
        {
            string xml = XmlSerialize(configuration);
            WriteConfiguration(xml, configuration.ConfigurationPath);
        }

        public static ISettingsConfiguration? LoadConfiguration(ISettingsConfiguration configuration)
        {
            if(!File.Exists(configuration.ConfigurationPath))
                return null;
            else
            {
                string xml = ReadConfiguration(configuration.ConfigurationPath).Result;
                return XmlDeserialize(xml, configuration.GetType());
            }
        }

        public static string XmlSerialize(object obj)
        {
            Type type = obj.GetType();
            IExtendedXmlSerializer serializer = new ConfigurationContainer()
                .UseOptimizedNamespaces()
                .EnableImplicitTyping(type)
                .Create();
            return serializer.Serialize(obj);
        }

        public static ISettingsConfiguration XmlDeserialize(string xml, Type type)
        {
            IExtendedXmlSerializer serializer = new ConfigurationContainer()
                .EnableImplicitTyping(new Type[]
                {
                    type,
                    typeof(AppUser)
                })
                .Create();
            return serializer.Deserialize<ISettingsConfiguration>(xml);
        }

        private static async Task<string> ReadConfiguration(string path)
        {
            return await File.ReadAllTextAsync(path);
        }
        private static async void WriteConfiguration(string xml, string path)
        {
            using StreamWriter writer = new StreamWriter(path);
            {
                writer.Write(xml);
                writer.Close();
            }            
        }
    }
}
