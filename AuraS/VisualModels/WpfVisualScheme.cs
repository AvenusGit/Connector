using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ConnectorCore.Models.VisualModels;
using ConnectorCore.Models.VisualModels.Interfaces;

namespace Aura.VisualModels
{
    public class WpfVisualScheme : IVisualScheme<WpfColorScheme,WpfFontScheme,WpfColorProperty>, IWpfScheme<WpfVisualScheme>
    {
        public WpfVisualScheme() { }
        public WpfVisualScheme(IVisualScheme<IColorScheme<string>,IFontScheme,string> scheme)
        {
            ColorScheme = new WpfColorScheme(scheme.ColorScheme);
            FontScheme = new WpfFontScheme(scheme.FontScheme);
        }
        public WpfVisualScheme(VisualScheme scheme)
        {
            ColorScheme = new WpfColorScheme(scheme.ColorScheme);
            FontScheme = new WpfFontScheme(scheme.FontScheme);
        }
        public new WpfColorScheme ColorScheme { get; set; }
        public new WpfFontScheme FontScheme { get; set; }

        public void Apply()
        {
            ColorScheme.Apply();
            FontScheme.Apply();
        }
        public WpfVisualScheme GetCurrent()
        {
            WpfVisualScheme currentVisualScheme = new WpfVisualScheme()
            {
                    ColorScheme = ColorScheme.GetCurrent(),
                    FontScheme = FontScheme.GetCurrent()
            };
            return currentVisualScheme;
        }
        public WpfVisualScheme Clone()
        {
            return new WpfVisualScheme()
            {
                ColorScheme = (WpfColorScheme)ColorScheme.Clone(),
                FontScheme = (WpfFontScheme)FontScheme.Clone()
            };
        }
        public WpfVisualScheme GetDefault()
        {
            return new WpfVisualScheme()
            {
                ColorScheme = new WpfColorScheme().GetDefault(),
                FontScheme = new WpfFontScheme(string.Empty,0).GetDefault()
            };
        }
        public VisualScheme ToVisualScheme()
        {
            return new VisualScheme()
            {
                ColorScheme = new ColorScheme()
                {
                    Fone = WpfColorProperty.WpfToCssColor("Fone", ColorScheme.Fone?.Color.ToString() ?? ""),
                    Accent = WpfColorProperty.WpfToCssColor("Accent", ColorScheme.Accent?.Color.ToString() ?? ""),
                    SubAccent = WpfColorProperty.WpfToCssColor("SubAccent", ColorScheme.SubAccent?.Color.ToString() ?? ""),
                    Panel = WpfColorProperty.WpfToCssColor("Panel", ColorScheme.Panel?.Color.ToString() ?? ""),
                    Border = WpfColorProperty.WpfToCssColor("Border", ColorScheme.Border?.Color.ToString() ?? ""),
                    Path = WpfColorProperty.WpfToCssColor("Path", ColorScheme.Path?.Color.ToString() ?? ""),
                    Text = WpfColorProperty.WpfToCssColor("Text", ColorScheme.Text?.Color.ToString() ?? ""),
                    Select = WpfColorProperty.WpfToCssColor("Select", ColorScheme.Select?.Color.ToString() ?? ""),
                    Error = WpfColorProperty.WpfToCssColor("Error", ColorScheme.Error?.Color.ToString() ?? ""),
                    Disable = WpfColorProperty.WpfToCssColor("Disable", ColorScheme.Disable?.Color.ToString() ?? ""),
                },
                FontScheme = new FontScheme()
                {
                    Font = FontScheme.Font,
                    FontMultiplierPercent = FontScheme.FontMultiplierPercent
                }
            };
        }
    }
}
