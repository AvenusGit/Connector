using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorCore.Interfaces
{
    public interface IServerInfo
    {
        public string Name { get; }
        public int Port { get; }
        public string HostOrIP { get; }
    }
}
