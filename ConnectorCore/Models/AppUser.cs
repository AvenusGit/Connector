using ConnectorCore.Models.Connections;
using ConnectorCore.Models.VisualModels;
using ConnectorCore.Models.VisualModels.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace ConnectorCore.Models
{
    public class AppUser
    {
        [XmlIgnore]
        [JsonIgnore]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? Name { get; set; }
        public Сredentials Credentials { get; set; }
        public List<AppUserGroup> Groups { get; set; } = new List<AppUserGroup>();
        public List<Connection> Connections { get; set; } = new List<Connection>();
        [XmlIgnore]
        [JsonIgnore]
        [NotMapped]
        public List<Connection> AllConnections
        {
            get
            {
                List<Connection> groupConnections = new List<Connection>();
                foreach (AppUserGroup group in Groups)
                {
                    groupConnections.AddRange(group.Connections);
                }
                return groupConnections.Concat(Connections).Distinct().ToList();
            }
        }
        public UserSettings? UserSettings { get; set; } = new UserSettings();
        public AppRoles Role { get; set; }
        public VisualScheme VisualScheme { get; set; } = VisualScheme.GetDefaultVisualScheme();
        public bool IsEnabled { get; set; }
        public enum AppRoles
        {
            User,
            Support,
            Administrator
        }
        public static AppUser GetDefault()
        {
            return new AppUser()
            {
                Name = "DefaultAdmin",
                Credentials = new Сredentials("connectorCenter", "connectorCenter"),
                Connections = new List<Connection>(),
                Role = AppUser.AppRoles.Administrator,
                UserSettings = UserSettings.GetDefault(),
                IsEnabled = true
            };
        }
    }
}
