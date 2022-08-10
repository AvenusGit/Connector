using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using AuraS.Interfaces;

namespace AuraS.Models
{
    public class ColorScheme : IColorScheme
    {
        public IColorProperty? Fone { get; set; }
        public IColorProperty? Accent { get; set; }
        public IColorProperty? SubAccent { get; set; }
        public IColorProperty? Panel { get; set; }
        public IColorProperty? Border { get; set; }
        public IColorProperty? Path { get; set; }
        public IColorProperty? Text { get; set; }
        public IColorProperty? Select { get; set; }
        public IColorProperty? Error { get; set; }
        public IColorProperty? Disable { get; set; }

        public Dictionary<string,IColorProperty> GetColorProperties()
        {
            Dictionary<string, IColorProperty> result = new Dictionary<string, IColorProperty>();
            result.Add("Fone", Fone!);
            result.Add("Accent", Accent!);
            result.Add("SubAccent", SubAccent!);
            result.Add("Panel", Panel!);
            result.Add("Border", Border!);
            result.Add("Path", Path!);
            result.Add("Text", Text!);
            result.Add("Select", Select!);
            result.Add("Error", Error!);
            result.Add("Disable", Disable!);
            return result;
        }

        public void Apply()
        {
            foreach (KeyValuePair<string, IColorProperty> property in GetColorProperties())
            {
                if(property.Value is ColorProperty)
                    (property.Value as ColorProperty).Apply();
            }         
        }
        public IColorScheme GetCurrent()
        {
            ColorScheme colorScheme = new ColorScheme();
            Type type = typeof(ColorScheme);
            foreach (string name in GetColorProperties().Keys)
            {
                ColorProperty property = ColorProperty.GetColorProperty(name);
                PropertyInfo? prop = type.GetProperty(name, typeof(ColorProperty));
                if (prop is not null)
                    prop.SetValue(colorScheme, property);
            }
            return colorScheme;
        }
        public IColorScheme Clone()
        {
            return new ColorScheme()
            {
                Fone = this.Fone,
                Accent = this.Accent,
                SubAccent = this.SubAccent,
                Border = this.Border,
                Path = this.Path,
                Text = this.Text,
                Select = this.Select,
                Error = this.Error,
                Disable = this.Disable
            };
        }
    }
}
