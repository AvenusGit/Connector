using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using AuraS.Models;
using AuraS.Interfaces;

namespace AuraS.Styles.DefaultStyles
{
    public static class DefaultStyleGenerator
    {
        public static IVisualScheme GetDefaultVisualScheme()
        {
            return new VisualScheme()
            {
                FontScheme = GetDefaultFontScheme(),
                ColorScheme = GetDefaultColorScheme()
            };
        }
        public static FontScheme GetDefaultFontScheme()
        {
            return new FontScheme()
            {
                Font = "Arial", //TODO
                FontMultiplierPercent = 100
            };
        }
        public static ColorScheme GetDefaultColorScheme()
        {
            return new ColorScheme()
            {
                Accent = new ColorProperty("Accent", (Color)ColorConverter.ConvertFromString("#FF363636")),
                Fone = new ColorProperty("Fone", (Color)ColorConverter.ConvertFromString("#FFFFFFFF")),
                SubAccent = new ColorProperty("SubAccent", (Color)ColorConverter.ConvertFromString("#FFADADAD")),
                Panel = new ColorProperty("Panel", (Color)ColorConverter.ConvertFromString("#FFFFFFFF")),
                Border = new ColorProperty("Border", (Color)ColorConverter.ConvertFromString("#FFDEDEDE")),
                Path = new ColorProperty("Path", (Color)ColorConverter.ConvertFromString("#FF3B3B3B")),
                Text = new ColorProperty("Text", (Color)ColorConverter.ConvertFromString("#FF3B3B3B")),
                Select = new ColorProperty("Select", (Color)ColorConverter.ConvertFromString("#FFDBDBDB")),
                Error = new ColorProperty("Error", (Color)ColorConverter.ConvertFromString("#AAFF0000")),
                Disable = new ColorProperty("Disable", (Color)ColorConverter.ConvertFromString("#FFF2F2F2")),
            };
        }
    }
}
