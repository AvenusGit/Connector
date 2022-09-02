using ConnectorCore.Models.Connections;
using ConnectorCore.Models.VisualModels;
using ConnectorCore.Models.VisualModels.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectorCore.Models
{
    public class AppUser : IAppUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? Name { get; set; }
        public Сredentials? Credentials { get; set; }
        public IEnumerable<UserGroup> Groups { get; set; } = new List<UserGroup>();
        public IEnumerable<Connection> Connections { get; set; } = new List<Connection>();
        public UserSettings? UserSettings { get; set; }
        public IAppUser.AppRoles Role { get; set; }
        public VisualScheme VisualScheme { get; set; }

        public static AppUser GetDefault()
        {
            return new AppUser()
            {
                Name = "DefaultAdmin",
                Credentials = new Сredentials("connectorCenter", "connectorCenter"),
                Connections = new List<Connection>(),
                Role = IAppUser.AppRoles.Administrator,
                UserSettings = UserSettings.GetDefault()
            };
        }
    }
}
