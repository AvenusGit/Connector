﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows;
using ConnectorCore.Models.VisualModels.Interfaces;
using ConnectorCore.Models.VisualModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Aura.VisualModels
{
    public class WpfColorProperty : ICloneable
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
                _color = value;
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
            if (Application.Current.Resources.Contains(ColorKeyName + "Color"))
                Application.Current.Resources[ColorKeyName + "Color"] = Color;
            else
                Application.Current.Resources.Add(ColorKeyName + "Color", Color);

            if (Application.Current.Resources.Contains(BrushKeyName))
                Application.Current.Resources[BrushKeyName] = BrushValue;
            else
                Application.Current.Resources.Add(BrushKeyName, BrushValue);
        }
        private static string CssToWpfColor(string wpfColor, string? colorName = null)
        {
            if (!IColorScheme<string>.IsValueCorrect(wpfColor))
                throw new Exception($"Ошибка при попытке перевода HEX цвета {colorName ?? "<" + colorName +">"} в WPF. Строка цвета не валидна");
            string opacity = wpfColor.Substring(wpfColor.Length - 2, 2);
            wpfColor = wpfColor.Remove(wpfColor.Length - 2, 2);
            return wpfColor.Insert(1, opacity);
        }
        public static Color GetColorFromString(string color)
        {
            return (Color)ColorConverter.ConvertFromString(CssToWpfColor(color));
        }
        public static string WpfToCssColor(string colorName, string wpfColor)
        {
            if (!IColorScheme<string>.IsValueCorrect(wpfColor))
                throw new Exception($"Ошибка при попытке перевода WPF цвета <{colorName}> в HEX. Строка цвета не валидна");
            string opacity = wpfColor.Substring(1, 2);
            wpfColor = wpfColor.Remove(1, 2);
            return wpfColor + opacity;
        }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
