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
                return XmlDeserialize<ISettingsConfiguration>(xml);
            }
        }

        public static string XmlSerialize(object obj)
        {
            try
            {
                Type type = obj.GetType();
                IExtendedXmlSerializer serializer = new ConfigurationContainer()
                    .UseOptimizedNamespaces()
                    .EnableImplicitTyping(type)
                    .Create();
                return serializer.Serialize(obj);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при попытке сериализации объекта {obj.GetType().Name}", ex);
            }
        }

        public static ObjectType XmlDeserialize<ObjectType>(string xml)
        {
            try
            {
                IExtendedXmlSerializer serializer = new ConfigurationContainer()
                                .EnableImplicitTyping(new Type[]
                                {
                                    typeof(ObjectType)
                                })
                                .Create();
                return serializer.Deserialize<ObjectType>(xml);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при попытке десериализации", ex);
            }            
        }

        private static async Task<string> ReadConfiguration(string path)
        {
            return await File.ReadAllTextAsync(path);
        }
        private static void WriteConfiguration(string xml, string path)
        {
            using StreamWriter writer = new StreamWriter(path);
            {
                writer.Write(xml);
                writer.Close();
            }            
        }
    }
}
