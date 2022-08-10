using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows;
using AuraS.Interfaces;

namespace AuraS.Models
{
    public class ColorProperty : IColorProperty
    {
        public ColorProperty(string name)
        {
            Name = name;
        }
        public ColorProperty(string name, Color color)
        {
            Name = name;
            ColorValue = color;
        }
        private Color? _color;
        public string Name { get; set; }
        public string ColorKeyName { get { return Name + "Color"; } }
        public Color? ColorValue 
        {
            get 
            {
                return _color;
            }
            set 
            {
                _color = value;
                Apply();
            }
        }
        public string BrushKeyName { get { return Name + "Brush"; } }
        public Brush? BrushValue 
        {
            get
            {
                if(ColorValue.HasValue)
                    return new SolidColorBrush(ColorValue.Value);
                else return null;
            }
        }

        public void Update()
        {
            if (Application.Current.Resources.Contains(ColorKeyName))
            {
                var resource = Application.Current.Resources[ColorKeyName];
                if(resource is Color)
                    ColorValue = (Color)resource;
            }   
            else
                ColorValue = null;
        }
        public static ColorProperty GetColorProperty(string name)
        {
            ColorProperty result = new ColorProperty(name);
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
