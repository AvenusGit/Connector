namespace ConnectorCenter.Models.Settings
{
    public class AccessSettings
    {
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

        public AccessModes SettingsAPI { get; set; }
        public AccessModes SettingsLogs { get; set; }
        public AccessModes SettingsOther { get; set; }
        public static AccessSettings GetUserDefault()
        {
            return new AccessSettings()
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
                SettingsAPI = AccessModes.None,
                SettingsLogs = AccessModes.None,
                SettingsOther = AccessModes.None
            };
        }
        public static AccessSettings GetSupportDefault()
        {
            return new AccessSettings()
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
                SettingsAPI = AccessModes.View,
                SettingsLogs = AccessModes.View,
                SettingsOther = AccessModes.View
            };
        }
        public static AccessSettings GetAdminDefault()
        {
            return new AccessSettings()
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
