using Aura.Models;
using System.Windows;
using System.Windows.Markup;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Media;

namespace Aura.Schemes.ThemeSchemes
{
    public static class DefaultThemeShemes
    {
        private static List<ThemeScheme> _defaultSchemes = new List<ThemeScheme>();

        public static List<ThemeScheme> DefaultThemeSchemes()
        {
            if (_defaultSchemes is null || _defaultSchemes.Count == 0)
            {
                InitializeDefaultThemeScheme();
                return _defaultSchemes;
            }
            else
            {
                return _defaultSchemes;
            }
        }

        private static void InitializeDefaultThemeScheme()
        {
            _defaultSchemes.Add(Standart());
            _defaultSchemes.Add(Dark());
            _defaultSchemes.Add(Original());
            _defaultSchemes.Add(Steel());
        }

        public static ThemeScheme Standart()
        {
            return new ThemeScheme
            {
                Name = "По умолчанию",
                FontScheme = Aura.Schemes.FontsSchemes.DefaultFontsShemes.Standart(),
                ColorScheme = Aura.Schemes.ColorsSchemes.DefaultColorShemes.StandartColorScheme(),
                StyleScheme = Aura.Schemes.Styles.StyleSchemes.AuraPreview(),
                IsServiceScheme = false
            };
        }

        public static ThemeScheme Dark()
        {
            return new ThemeScheme
            {
                Name = "Темная",
                FontScheme = Aura.Schemes.FontsSchemes.DefaultFontsShemes.Dark(),
                ColorScheme = Aura.Schemes.ColorsSchemes.DefaultColorShemes.DarkColorScheme(),
                StyleScheme = Aura.Schemes.Styles.StyleSchemes.AuraPreview(),
                IsServiceScheme = false
            };
        }

        public static ThemeScheme Original()
        {
            return new ThemeScheme
            {
                Name = "Оригинальная",
                FontScheme = Aura.Schemes.FontsSchemes.DefaultFontsShemes.Original(),
                ColorScheme = Aura.Schemes.ColorsSchemes.DefaultColorShemes.OriginalColorScheme(),
                StyleScheme = Aura.Schemes.Styles.StyleSchemes.AuraQuad(),
                IsServiceScheme = false
            };
        }

        public static ThemeScheme Steel()
        {
            return new ThemeScheme
            {
                Name = "Сталь",
                FontScheme = Aura.Schemes.FontsSchemes.DefaultFontsShemes.Steel(),
                ColorScheme = Aura.Schemes.ColorsSchemes.DefaultColorShemes.SteelColorScheme(),
                StyleScheme = Aura.Schemes.Styles.StyleSchemes.AuraQuad(),
                IsServiceScheme = false
            };
        }
    }
}
