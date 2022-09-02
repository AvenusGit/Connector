using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectorCore.Models.Connections;

namespace ConnectorCore.Models
{
    public class Server
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? Name { get; set; }
        public string Host { get; set; }
        public bool IsAvailable { get; set; }
        public List<Connection> Connections { get; set; } = new List<Connection>();
    }
}
