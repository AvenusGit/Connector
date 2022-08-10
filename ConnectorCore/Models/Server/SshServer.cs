using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectorCore.Interfaces;
using ConnectorCore.Models.Server;
using ConnectorCore.Models.Connections;

namespace ConnectorCore.Models.Server
{
    public class SshServer : IServer
    {
        public ServerInfo ServerInfo { get; set; }
        public IEnumerable<SshConnection> SshConnections { get; set; }
    }
}
