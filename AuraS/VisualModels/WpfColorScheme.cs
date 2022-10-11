using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using ConnectorCore.Models.VisualModels.Interfaces;
using ConnectorCore.Models.VisualModels;

namespace Aura.VisualModels
{
    public class WpfColorScheme : IColorScheme<WpfColorProperty>, IWpfScheme<WpfColorScheme>, ICloneable
    {
        public WpfColorScheme() { }
        public WpfColorScheme(IColorScheme<string> colorScheme)
        {
            Fone = new WpfColorProperty("Fone", colorScheme.Fone);
            Accent = new WpfColorProperty("Accent", colorScheme.Accent);
            SubAccent = new WpfColorProperty("SubAccent", colorScheme.SubAccent);
            Panel = new WpfColorProperty("Panel", colorScheme.Panel);
            Border = new WpfColorProperty("Border", colorScheme.Border);
            Path = new WpfColorProperty("Path", colorScheme.Path);
            Text = new WpfColorProperty("Text", colorScheme.Text);
            Select = new WpfColorProperty("Select", colorScheme.Select);
            Error = new WpfColorProperty("Error", colorScheme.Error);
            Disable = new WpfColorProperty("Disable", colorScheme.Disable);
        }
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
            foreach (KeyValuePair<string, WpfColorProperty> property in GetColorProperties())
            {
                if (property.Value is WpfColorProperty)
                    (property.Value).Apply();
                    
            }
        }
        public Dictionary<string, WpfColorProperty> GetColorProperties()
        {
            Dictionary<string, WpfColorProperty> result = new Dictionary<string, WpfColorProperty>();
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
        public WpfColorScheme GetFromColorScheme(ColorScheme colorScheme)
        {
            return new WpfColorScheme()
            {
                Fone = new WpfColorProperty("Fone", colorScheme.Fone),
                Accent = new WpfColorProperty("Accent", colorScheme.Accent),
                SubAccent = new WpfColorProperty("SubAccent", colorScheme.SubAccent),
                Panel = new WpfColorProperty("Panel", colorScheme.Panel),
                Border = new WpfColorProperty("Border", colorScheme.Border),
                Path = new WpfColorProperty("Path", colorScheme.Path),
                Text = new WpfColorProperty("Text", colorScheme.Text),
                Select = new WpfColorProperty("Select", colorScheme.Select),
                Error = new WpfColorProperty("Error", colorScheme.Error),
                Disable = new WpfColorProperty("Disable", colorScheme.Disable)
            };
        }
        public WpfColorScheme GetDefault()
        {
            return GetFromColorScheme(ColorScheme.GetDefault());            
        }
        public object Clone()
        {
            return new WpfColorScheme()
            {
                Fone = (WpfColorProperty?)Fone?.Clone(),
                Accent = (WpfColorProperty?)Accent?.Clone(),
                SubAccent = (WpfColorProperty?)SubAccent?.Clone(),
                Panel = (WpfColorProperty?)Panel?.Clone(),
                Border = (WpfColorProperty?)Border?.Clone(),
                Path = (WpfColorProperty?)Path?.Clone(),
                Text = (WpfColorProperty?)Text?.Clone(),
                Select = (WpfColorProperty?)Select?.Clone(),
                Error = (WpfColorProperty?)Error?.Clone(),
                Disable = (WpfColorProperty?)Disable?.Clone()
            };
        }
    }
}
