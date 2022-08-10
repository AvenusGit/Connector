using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectorCore.Models.Authorization;

namespace ConnectorCore.Interfaces
{
    public interface IConnectionInfo
    {
        public string ConnectionName { get; set; }
        public string UserName { get; set; }
        public Сredentials Сredentials { get; set; }
        public IServerInfo ServerInfo { get; set; }
    }
}
