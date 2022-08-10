using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorCore.Interfaces
{
    public interface ISettings
    {
        public IServerInfo MainServer { get; set; }
    }
}
