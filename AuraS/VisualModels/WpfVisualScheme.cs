using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ConnectorCore.Models.VisualModels;
using ConnectorCore.Models.VisualModels.Interfaces;

namespace AuraS.VisualModels
{
    public class WpfVisualScheme : VisualScheme
    {
        public new WpfColorScheme ColorScheme { get; set; }
        public new WpfFontScheme FontScheme { get; set; }

        public void Apply()
        {
            ColorScheme.Apply();
            FontScheme.Apply();
        }
        public static WpfVisualScheme GetCurrent()
        {
            WpfVisualScheme currentVisualScheme = new WpfVisualScheme();
            currentVisualScheme.ColorScheme = currentVisualScheme.ColorScheme.GetCurrent();
            currentVisualScheme.FontScheme = currentVisualScheme.FontScheme.GetCurrent();
            return currentVisualScheme;
        }
        public WpfVisualScheme Clone()
        {
            return new WpfVisualScheme()
            {
                ColorScheme = ColorScheme,
                FontScheme = FontScheme
            };
        }
    }
}
