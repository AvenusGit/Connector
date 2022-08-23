using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using ConnectorCore.Models.VisualModels.Interfaces;
using ConnectorCore.Models.VisualModels;

namespace AuraS.VisualModels
{
    public class WpfColorScheme : ColorScheme
    {
        public new WpfColorProperty? Fone { get; set; }
        public new WpfColorProperty? Accent { get; set; }
        public new WpfColorProperty? SubAccent { get; set; }
        public new WpfColorProperty? Panel { get; set; }
        public new WpfColorProperty? Border { get; set; }
        public new WpfColorProperty? Path { get; set; }
        public new WpfColorProperty? Text { get; set; }
        public new WpfColorProperty? Select { get; set; }
        public new WpfColorProperty? Error { get; set; }
        public new WpfColorProperty? Disable { get; set; }

        public void Apply()
        {
            foreach (KeyValuePair<string, IColorProperty> property in GetColorProperties())
            {
                if (property.Value is WpfColorProperty)
                    (property.Value as WpfColorProperty).Apply();
            }
        }
        public WpfColorScheme GetCurrent()
        {
            WpfColorScheme colorScheme = new WpfColorScheme();
            Type type = typeof(WpfColorScheme);
            foreach (string name in GetColorProperties().Keys)
            {
                WpfColorProperty property = WpfColorProperty.GetColorProperty(name);
                PropertyInfo? prop = type.GetProperty(name, typeof(WpfColorProperty));
                if (prop is not null)
                    prop.SetValue(colorScheme, property);
            }
            return colorScheme;
        }
    }
}
