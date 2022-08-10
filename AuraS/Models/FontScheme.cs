using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using AuraS.Interfaces;

namespace AuraS.Models
{
    public class FontScheme : IScheme<IFontScheme>, IFontScheme
    {
        public string Font { get; set; }
        public double? FontMultiplierPercent { get; set; }
        public IFontScheme GetCurrent()
        {
            FontScheme result = new FontScheme();
            if(Application.Current.Resources.Contains("FontFamily"))
                result.Font = ((FontFamily)Application.Current.Resources["FontFamily"]).Source;
            if (Application.Current.Resources.Contains("FontMultiplier"))
                result.FontMultiplierPercent = (double)Application.Current.Resources["FontMultiplier"];
            return result;
        }
        public void Apply()
        {
            foreach (string key in Application.Current.Resources.Keys)
            {
                if(key.StartsWith("FontSize"))
                {
                    double withoutMultiplier;
                    if (double.TryParse(key.Replace("FontSize", ""), out withoutMultiplier))
                        Application.Current.Resources[key] = withoutMultiplier * FontMultiplierPercent / 100;
                    else continue;
                }
                      
            }
            Application.Current.Resources["Font"] = Font;
            Application.Current.Resources["FontMultiplier"] = FontMultiplierPercent ?? 100;
        }
        public IFontScheme Clone()
        {
            return new FontScheme()
            {
                Font = this.Font,
                FontMultiplierPercent = this.FontMultiplierPercent,
            };
        }
    }
}
