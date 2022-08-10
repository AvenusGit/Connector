using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectorCore.Interfaces;
using ConnectorCore.Models.Connections;

namespace ConnectorCore.Models.Server
{
    public class RdpServer : IServer
    {
        public ServerInfo ServerInfo { get; set; }
        public IEnumerable<RdpConnection> RdpConnections { get; set; }
    }
}
