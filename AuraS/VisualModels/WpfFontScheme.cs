using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ConnectorCore.Models.VisualModels;
using ConnectorCore.Models.VisualModels.Interfaces;

namespace AuraS.VisualModels
{
    public class WpfFontScheme : FontScheme
    {
        public FontFamily FontValue
        {
            get
            {
                return new FontFamily(Font);
            }
            set
            {
                Font = value.Source;
            }
        }
        public WpfFontScheme GetCurrent()
        {
            WpfFontScheme result = new WpfFontScheme();
            if (Application.Current.Resources.Contains("Font"))
                result.Font = ((FontFamily)Application.Current.Resources["Font"]).Source;
            if (Application.Current.Resources.Contains("FontMultiplier"))
                result.FontMultiplierPercent = (double)Application.Current.Resources["FontMultiplier"];
            return result;
        }
        public void Apply()
        {
            foreach (string key in Application.Current.Resources.Keys)
            {
                if (key.StartsWith("FontSize"))
                {
                    double withoutMultiplier;
                    if (double.TryParse(key.Replace("FontSize", ""), out withoutMultiplier))
                        Application.Current.Resources[key] = withoutMultiplier * FontMultiplierPercent / 100;
                    else continue;
                }

            }
            Application.Current.Resources["Font"] = FontValue;
            Application.Current.Resources["FontMultiplier"] = FontMultiplierPercent ?? 100;
        }
    }
}
