using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectorCore.Models.Authorization;

namespace ConnectorCore.Interfaces
{
    public interface IConnection
    {
        public string ConnectionName { get; set; }
        public string ConnectionDescription { get;}
        public bool Locked { get; set; }
    }
}
