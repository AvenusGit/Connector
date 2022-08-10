using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectorCore.Interfaces;

namespace ConnectorCore.Models.Server
{
    public class ServerInfo :IServerInfo
    {
        public ServerInfo() { }
        public ServerInfo(string name, int port, string hostName)
        {
            Name = name;
            Port = port;
            HostOrIP = hostName;
        }

        public string Name { get; set; }
        public int Port { get; set; }
        public string HostOrIP { get; set; }
    }
}
