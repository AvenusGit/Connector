using ConnectorCore.Models;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace ConnectorCenter.Models.Settings
{
    public class AccessSettings : ISettingsConfiguration
    {
        private AppUser.AppRoles _targetRole;
        public AccessSettings()
        {
            _targetRole = AppUser.AppRoles.User;
        }
        public AccessSettings(AppUser.AppRoles targetRole)
        {
            _targetRole = targetRole;
        }
        [XmlIgnore]
        [JsonIgnore]
        public string ConfigurationPath 
        {
            get
            {
                switch (_targetRole)
                {
                    case AppUser.AppRoles.Administrator:
                        return ConnectorCenterApp.Instance.ConfigurationFolderPath + @"\AdminAccess.config";
                    case AppUser.AppRoles.Support:
                        return ConnectorCenterApp.Instance.ConfigurationFolderPath + @"\SupportAccess.config";
                    case AppUser.AppRoles.User:
                        return ConnectorCenterApp.Instance.ConfigurationFolderPath + @"\UserAccess.config";
                    default:
                        throw new Exception("При запросе пути конфигурации обнаружена неопределенная роль пользователя.");
                }
            }
        }
        public bool WebAccess { get; set; }

        public AccessModes Groups { get; set; }
        public AccessModes GroupConnections { get; set; }
        public AccessModes GroupUsers { get; set; }

        public AccessModes Servers { get; set; }
        public AccessModes ServersConnections { get; set; }

        public AccessModes Users { get; set; }
        public AccessModes UserConnections { get; set; }

        public bool Statistics { get; set; }
        public bool Logs { get; set; }
        public bool ImportAndExport { get; set; }

        public AccessModes SettingsAPI { get; set; }
        public AccessModes SettingsLogs { get; set; }
        public AccessModes SettingsOther { get; set; }
        public AccessSettings GetDefault()
        {
            switch (_targetRole)
            {
                case AppUser.AppRoles.User:
                    return GetUserDefault();
                case AppUser.AppRoles.Support:
                    return GetSupportDefault();
                case AppUser.AppRoles.Administrator:
                    return GetAdminDefault();
                default:
                    throw new Exception("Ошибка при получении настроек по умолчанию. Нераспознанная роль пользователя.");
            }
        }
        public static AccessSettings GetUserDefault()
        {
            return new AccessSettings(AppUser.AppRoles.User)
            {
                WebAccess = false,
                Groups = AccessModes.None,
                GroupConnections = AccessModes.None,
                GroupUsers = AccessModes.None,
                Servers = AccessModes.None,
                ServersConnections = AccessModes.None,
                Users = AccessModes.None,
                UserConnections = AccessModes.None,
                Statistics = false,
                Logs = false,
                ImportAndExport = false,
                SettingsAPI = AccessModes.None,
                SettingsLogs = AccessModes.None,
                SettingsOther = AccessModes.None
            };
        }
        public static AccessSettings GetSupportDefault()
        {
            return new AccessSettings(AppUser.AppRoles.Support)
            {
                WebAccess = true,
                Groups = AccessModes.View,
                GroupConnections = AccessModes.Edit,
                GroupUsers = AccessModes.Edit,
                Servers = AccessModes.View,
                ServersConnections = AccessModes.View,
                Users = AccessModes.View,
                UserConnections = AccessModes.Edit,
                Statistics = true,
                Logs = true,
                ImportAndExport = false,
                SettingsAPI = AccessModes.View,
                SettingsLogs = AccessModes.View,
                SettingsOther = AccessModes.View
            };
        }
        public static AccessSettings GetAdminDefault()
        {
            return new AccessSettings(AppUser.AppRoles.Administrator)
            {
                WebAccess = true,
                Groups = AccessModes.Edit,
                GroupConnections = AccessModes.Edit,
                GroupUsers = AccessModes.Edit,
                Servers = AccessModes.Edit,
                ServersConnections = AccessModes.Edit,
                Users = AccessModes.Edit,
                UserConnections = AccessModes.Edit,
                Statistics = true,
                Logs = true,
                ImportAndExport = true,
                SettingsAPI = AccessModes.Edit,
                SettingsLogs = AccessModes.Edit,
                SettingsOther = AccessModes.Edit
            };
        }
        public enum AccessModes
        {
            None,
            View,
            Edit
        }
    }
}
