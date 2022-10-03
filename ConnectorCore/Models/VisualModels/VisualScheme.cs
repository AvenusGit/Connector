using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using ConnectorCore.Models.VisualModels.Interfaces;

namespace ConnectorCore.Models.VisualModels
{
    public class VisualScheme : IVisualScheme<ColorScheme, FontScheme>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
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

        public string GenerateCss(IVisualScheme<IColorScheme,IFontScheme> scheme)
        {
            return "";
        }
    }
}
