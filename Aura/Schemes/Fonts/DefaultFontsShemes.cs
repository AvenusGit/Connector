using Aura.Models;
using System.Windows;
using System.Windows.Markup;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Media;

namespace Aura.Schemes.FontsSchemes
{
    public static class DefaultFontsShemes
    {
        private static List<FontScheme> _defaultSchemes = new List<FontScheme>();

        public static List<FontScheme> DefaultFontSchemes()
        {
            if (_defaultSchemes is null || _defaultSchemes.Count == 0)
            {
                InitializeDefaultFontScheme();
                return _defaultSchemes;
            }
            else
            {
                return _defaultSchemes;
            }
        }

        private static void InitializeDefaultFontScheme()
        {
            foreach (ThemeScheme theme in Aura.Schemes.ThemeSchemes.DefaultThemeShemes.DefaultThemeSchemes())
            {
                _defaultSchemes.Add(theme.FontScheme);
            }
        }

        public static FontScheme Standart()
        {
            return new FontScheme
            {
                Name = "По умолчанию",
                Font = new FontFamily("Yu Gothic UI Light"),
                Multiplier = 100,
                IsServiceScheme = false
            };
        }

        public static FontScheme Dark()
        {
            return new FontScheme
            {
                Name = "Темная",
                Font = new FontFamily("Iosevka"),
                Multiplier = 100,
                IsServiceScheme = false
            };
        }

        public static FontScheme Original()
        {
            return new FontScheme
            {
                Name = "Оригинальная",
                Font = new FontFamily("Arial"),
                Multiplier = 100,
                IsServiceScheme = false
            };
        }

        public static FontScheme Steel()
        {
            return new FontScheme
            {
                Name = "Сталь",
                Font = new FontFamily("Iosevka"),
                Multiplier = 100,
                IsServiceScheme = false
            };
        }
    }
}
