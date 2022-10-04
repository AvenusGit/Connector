using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ConnectorCore.Models.Connections;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace ConnectorCore.Models
{
    public class AppUserGroup
    {
        [XmlIgnore]
        [JsonIgnore]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? GroupName { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public List<AppUser> Users { get; set; } = new List<AppUser>();
        public List<Connection> Connections { get; set; } = new List<Connection>();
    }
}
