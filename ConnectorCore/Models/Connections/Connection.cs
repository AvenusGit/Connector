using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorCore.Models.Connections
{
    public class Connection
    {
        public long Id { get; set; }
        public string? ConnectionName { get; set; }
        public string ConnectionDescription
        {
            get
            {
                return $"{ConnectionType.ToString()} ({Server.Host}:{Port})/{User?.Name}";
            }
        }
        public bool IsAvailable { get; set; }
        public int Port { get; set; }
        public Server Server { get; set; }
        public ServerUser? User { get; set; }
        public ConnectionTypes ConnectionType { get; set; }
        public enum ConnectionTypes
        {
            SSH,
            RDP
        }
    }
}
