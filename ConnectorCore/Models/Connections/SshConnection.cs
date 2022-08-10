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
    public class SshConnection : IConnection
    {
        public string ConnectionName { get; set; }
        public string ConnectionDescription
        {
            get
            {
                return $"SSH:{Server.HostOrIP} : {Сredentials.Login}";
            }
        }
        public bool Locked { get; set; }
        public Сredentials Сredentials { get; set; }
        public ServerInfo Server{ get; set; }
    }
}
