using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using ConnectorCore.Models.VisualModels.Interfaces;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace ConnectorCore.Models.VisualModels
{
    public class VisualScheme : IVisualScheme<ColorScheme, FontScheme, string>
    {
        [XmlIgnore]
        [JsonIgnore]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public long AppUserId { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public AppUser AppUser { get; set; }
        public ColorScheme ColorScheme { get; set; }
        public FontScheme FontScheme { get; set; }
        public static VisualScheme GetDefaultVisualScheme()
        {
            return new VisualScheme()
            {
                ColorScheme = ColorScheme.GetDefault(),
                FontScheme = new FontScheme()
                {
                    Font = "Yu Gothic UI Light", //TODO
                    FontMultiplierPercent = 100
                }
            };
        }
    }
}
