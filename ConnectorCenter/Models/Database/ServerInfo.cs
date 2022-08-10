using ConnectorCore.Models.Server;
using ConnectorCore.Interfaces;
using System.Data.Entity;
using ConnectorCore.Interfaces;
namespace ConnectorCenter.Models.Database
{
    public class ServerInfo : IServerInfo
    {
        public string Name { get; set; }
        public int Port { get; set; }
        public string HostOrIP { get; set; }
        public string Description { get; set; }
        public IEnumerable<IConnection> Connections { get; set; }

    }
    public class ServerInfoContext : DbContext
    {
        public ServerInfoContext() : base("DefaultConnection") { }
        public DbSet<ServerInfo> ServerInfos { get; set; }
    }
}
