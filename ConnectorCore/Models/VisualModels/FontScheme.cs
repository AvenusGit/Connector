using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ConnectorCore.Models.VisualModels.Interfaces;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace ConnectorCore.Models.VisualModels
{
    public class FontScheme : IFontScheme
    {
        [XmlIgnore]
        [JsonIgnore]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Font { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public long VisualSchemeId { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public VisualScheme VisualScheme { get; set; }
        public double? FontMultiplierPercent { get; set; }
       
        public IFontScheme Clone()
        {
            return new FontScheme()
            {
                Font = Font,
                FontMultiplierPercent = FontMultiplierPercent,
            };
        }
    }
}
