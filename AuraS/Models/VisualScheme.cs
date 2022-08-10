using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuraS.Interfaces;

namespace AuraS.Models
{
    public class VisualScheme : IVisualScheme
    {
        public IColorScheme ColorScheme{ get; set; }
        public IFontScheme FontScheme { get; set; }

        public void Apply()
        {
            ColorScheme.Apply();
            FontScheme.Apply();
        }
        public IVisualScheme GetCurrent()
        {
            VisualScheme currentVisualScheme = new VisualScheme();
            currentVisualScheme.ColorScheme = ColorScheme.GetCurrent();
            currentVisualScheme.FontScheme = FontScheme.GetCurrent();
            return currentVisualScheme;
        }
        public IVisualScheme Clone()
        {
            return new VisualScheme()
            {
                ColorScheme = this.ColorScheme,
                FontScheme = this.FontScheme
            };
        }
    }
}
