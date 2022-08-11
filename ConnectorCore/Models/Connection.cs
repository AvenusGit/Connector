namespace ConnectorCore.Models
{
    public class Connection
    {
        public long Id { get; set; }
        public string? ConnectionName { get; set; }
        public string ConnectionDescription
        {
            get
            {
                switch (ConnectionType)
                {
                    case ConnectionTypes.RDP:
                        return $"{ConnectionType}:{Server.Host} / {User.Name}({User.Role})";
                    case ConnectionTypes.SSH:
                        return $"{ConnectionType}:{Server.Host} / {User.Credentials.Login}";
                }
                return $"{Server.Host}";
            }
        }
        public bool? Locked { get; set; }
        public ConnectionTypes ConnectionType { get; set; }
        public ServerInfo? Server { get; set; }
        public ServerUser? User { get; set; }
        public enum ConnectionTypes
        {
            RDP,
            SSH
        }
    }
}
