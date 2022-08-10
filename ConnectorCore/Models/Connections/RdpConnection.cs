using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectorCore.Interfaces;
using ConnectorCore.Models.Authorization;
using ConnectorCore.Models.Server;

namespace ConnectorCore.Models.Connections
{
    public class RdpConnection : IConnection
    {
        public string ConnectionName { get; set; }
        public string ConnectionDescription 
        {
            get
            {
                return $"RDP:{Server.HostOrIP} as {User.Name}({User.Role})";
            }
        }
        public bool Locked { get; set; }
        public ServerInfo Server { get; set; }
        public RdpUser User { get; set; }
    }
}
