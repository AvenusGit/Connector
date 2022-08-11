using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorCore.Models
{
    public class Server
    {
        public long Id { get; set; }
        public ServerGroup? ServerGroup { get; set; }
        public string? Name { get; set; }
        public ServerInfo? ServerInfo { get; set; }
        public IEnumerable<Connection> Connections { get; set; } = new List<Connection>();
    }
}
