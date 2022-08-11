using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorCore.Models
{
    public class ServerInfo
    {
        public long Id { get; set; }
        public string? Host { get; set; }
        public int? Port { get; set; }
    }
}
