using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace ConnectorCore.Models.Connections
{
    public class Connection
    {
        [XmlIgnore]
        [JsonIgnore]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? ConnectionName { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public string ConnectionDescription
        {
            get
            {
                return $"{ConnectionType.ToString()}:{Server.Name}/{ServerUser?.Name}";
            }
        }
        public string ConnectionUserShort
        {
            get
            {
                return ToString();
            }            
        }
        public bool IsAvailable { get; set; }
        [XmlIgnore]
        //[JsonIgnore]
        public Server Server { get; set; }
        public ServerUser ServerUser { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public List<AppUser> AppUsers { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public List<AppUserGroup> AppUsersGroups { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public int Port
        {
            get
            {
                switch (ConnectionType)
                {
                    case ConnectionTypes.SSH:
                        return Server.SshPort;
                    case ConnectionTypes.RDP:
                        return Server.RdpPort;
                    default: throw new Exception("Неизвестный тип подключения");
                }
            }
        }
        public ConnectionTypes ConnectionType { get; set; }
        public enum ConnectionTypes
        {
            SSH,
            RDP
        }
        public override string ToString()
        {
            return ServerUser.Name + "@" + Server.Name;
        }
    }
}
