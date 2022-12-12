using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace ConnectorCore.Models
{
    public class UserSettings
    {
        [XmlIgnore]
        [JsonIgnore]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [XmlIgnore]
        [ JsonIgnore]
        public long AppUserId { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public AppUser AppUser { get; set; }
        public string HelloText { get; set; } = "Hello!";
        public RdpSettings RdpSettings { get; set; } = RdpSettings.GetDefault();
        public static UserSettings GetDefault()
        {
            return new UserSettings()
            {
                HelloText = "",
                RdpSettings = RdpSettings.GetDefault()
            };
        }
    }
}
