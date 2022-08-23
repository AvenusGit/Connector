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
                ColorScheme = new ColorScheme()
                {
                    Accent = new ColorProperty("Accent", "#FF363636"),
                    Fone = new ColorProperty("Fone", "#FFFFFFFF"),
                    SubAccent = new ColorProperty("SubAccent",  "#FFADADAD"),
                    Panel = new ColorProperty("Panel",  "#FFFFFFFF"),
                    Border = new ColorProperty("Border",  "#FFDEDEDE"),
                    Path = new ColorProperty("Path",  "#FF3B3B3B"),
                    Text = new ColorProperty("Text",  "#FF3B3B3B"),
                    Select = new ColorProperty("Select",  "#FFDBDBDB"),
                    Error = new ColorProperty("Error",  "#AAFF0000"),
                    Disable = new ColorProperty("Disable",  "#FFF2F2F2"),
                },
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
