using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows;
using ConnectorCore.Models.VisualModels.Interfaces;
using ConnectorCore.Models.VisualModels;

namespace AuraS.VisualModels
{
    public class WpfColorProperty : ColorProperty
    {
        public WpfColorProperty(string name)
        {
            Name = name;
        }
        public WpfColorProperty(string name, string color)
        {
            Name = name;
            Color = color;
        }
        public Color? ColorValue
        {
            get
            {
                return (Color)ColorConverter.ConvertFromString(Color); ;
            }
            set
            {
                Color = value is null ? "#FFFFFF" : value.ToString();
                Apply();
            }
        }
        public string BrushKeyName { get { return Name + "Brush"; } }
        public Brush? BrushValue
        {
            get
            {
                if (ColorValue.HasValue)
                    return new SolidColorBrush(ColorValue.Value);
                else return null;
            }
        }

        public void Update()
        {
            if (Application.Current.Resources.Contains(ColorKeyName))
            {
                var resource = Application.Current.Resources[ColorKeyName];
                if (resource is Color)
                    ColorValue = (Color)resource;
            }
            else
                ColorValue = null;
        }
        public static WpfColorProperty GetColorProperty(string name)
        {
            WpfColorProperty result = new WpfColorProperty(name);
            result.Update();
            return result;
        }
        public void Apply()
        {
            if (Application.Current.Resources.Contains(ColorKeyName))
                Application.Current.Resources[ColorKeyName] = ColorValue;
            else
                Application.Current.Resources.Add(ColorKeyName, ColorValue);

            if (Application.Current.Resources.Contains(BrushKeyName))
                Application.Current.Resources[BrushKeyName] = BrushValue;
            else
                Application.Current.Resources.Add(BrushKeyName, BrushValue);
        }
    }
}
