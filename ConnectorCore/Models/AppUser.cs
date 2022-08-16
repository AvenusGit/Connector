namespace ConnectorCore.Models
{
    public class AppUser
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public Сredentials? Credentials { get; set; }
        public IEnumerable<Connection> Connections { get; set; } = new List<Connection>();
        public UserSettings? UserSettings { get; set; }
        public AppUser.AppRoles Role { get; set; }
        public enum AppRoles
        {
            User,
            Support,
            Administrator
        }

        public static AppUser GetDefault()
        {
            return new AppUser()
            {
                Name = "DefaultAdmin",
                Credentials = new Сredentials("connectorCenter", "connectorCenter"),
                Connections = new List<Connection>(),
                Role = AppRoles.Administrator,
                UserSettings = UserSettings.GetDefault()
            };
        }
    }
}
