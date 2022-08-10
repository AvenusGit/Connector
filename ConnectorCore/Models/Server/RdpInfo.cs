using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectorCore.Interfaces;
using ConnectorCore.Models.Authorization;

namespace ConnectorCore.Models.Server
{
    public class RdpInfo : IConnectionInfo
    {
        public string ConnectionName { get; set; }
        public string UserName { get; set; }
        public Сredentials Сredentials { get; set; }
        public IServerInfo ServerInfo { get; set; }
        public IUser.Roles Role { get; set; }
    }
}
