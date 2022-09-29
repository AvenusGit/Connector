using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConnectorCore.Models
{
    public class UserSettings
    {
        [XmlIgnoreAttribute]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [XmlIgnoreAttribute]
        public long AppUserId { get; set; }
        [XmlIgnoreAttribute]
        public AppUser AppUser { get; set; }
        // some user settings
        public string HelloText { get; set; } = "Hello!";
        public static UserSettings GetDefault()
        {
            return new UserSettings();
        }
    }
}
