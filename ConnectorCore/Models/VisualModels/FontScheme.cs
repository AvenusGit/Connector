using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ConnectorCore.Models.VisualModels.Interfaces;

namespace ConnectorCore.Models.VisualModels
{
    public class FontScheme : IFontScheme
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Font { get; set; }
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
