using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectorCore.Models.Connections;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace ConnectorCore.Models
{
    public class Server
    {
        [XmlIgnore]
        [JsonIgnore]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? Name { get; set; }
        public string Host { get; set; }
        public int RdpPort { get; set; } = 3389;
        public int SshPort { get; set; } = 22;
        public bool IsAvailable { get; set; }
        public List<Connection> Connections { get; set; } = new List<Connection>();
    }
}
