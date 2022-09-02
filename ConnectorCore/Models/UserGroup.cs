using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ConnectorCore.Models.Connections;

namespace ConnectorCore.Models
{
    public class UserGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? GroupName { get; set; }
        public IEnumerable<Connection> Servers { get; set; } = new List<Connection>();
    }
}
