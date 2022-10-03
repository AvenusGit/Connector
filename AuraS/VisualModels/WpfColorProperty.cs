using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows;
using ConnectorCore.Models.VisualModels.Interfaces;
using ConnectorCore.Models.VisualModels;

namespace AuraS.VisualModels
{
    public class WpfColorProperty
    {
        public WpfColorProperty(string name)
        {
            ColorKeyName = name;
            Color = GetColorFromString("#FFFFFFFF");
        }
        public WpfColorProperty(string name, Color color)
        {
            ColorKeyName = name;
            Color = color;
        }
        public WpfColorProperty(string name, string color)
        {
            ColorKeyName = name;
            Color = GetColorFromString(color);
        }
        public string ColorKeyName { get; set; }
        private Color _color;
        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                Color = value;
                Apply();
            }
        }
        public string BrushKeyName { get { return ColorKeyName + "Brush"; } }
        public Brush? BrushValue
        {
            get
            {
                    return new SolidColorBrush(Color);
            }
        }

        public void Update()
        {
            if (Application.Current.Resources.Contains(ColorKeyName))
            {
                var resource = Application.Current.Resources[ColorKeyName];
                if (resource is Color)
                    Color = (Color)resource;
            }
            else
                Color = GetColorFromString("#FFFFFFFF");
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
                Application.Current.Resources[ColorKeyName] = Color;
            else
                Application.Current.Resources.Add(ColorKeyName, Color);

            if (Application.Current.Resources.Contains(BrushKeyName))
                Application.Current.Resources[BrushKeyName] = BrushValue;
            else
                Application.Current.Resources.Add(BrushKeyName, BrushValue);
        }
        public static Color GetColorFromString(string color)
        {
            return (Color)ColorConverter.ConvertFromString(color);
        }
    }
}
