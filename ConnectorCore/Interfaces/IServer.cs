using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectorCore.Models.Server;

namespace ConnectorCore.Interfaces
{
    public interface IServer
    {
        public ServerInfo ServerInfo { get; set; }
    }
}
