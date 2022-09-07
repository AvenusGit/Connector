using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ConnectorCore.Models.Connections;

namespace ConnectorCore.Models
{
    public class AppUserGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? GroupName { get; set; }
        public List<AppUser> Users { get; set; } = new List<AppUser>();
        public List<Connection> Connections { get; set; } = new List<Connection>();
    }
}
