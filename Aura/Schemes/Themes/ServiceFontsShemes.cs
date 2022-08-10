using Aura.Models;
using System.Windows;
using System.Windows.Markup;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Media;

namespace Aura.Schemes.ThemeSchemes
{
    public static class ServiceThemeShemes
    {
        private static List<ThemeScheme> _serviceSchemes = new List<ThemeScheme>();

        public static List<ThemeScheme> ServiceThemeSchemes()
        {
            if (_serviceSchemes is null || _serviceSchemes.Count == 0)
            {
                InitializeServiceThemeScheme();
                return _serviceSchemes;
            }
            else
            {
                return _serviceSchemes;
            }
        }

        private static void InitializeServiceThemeScheme()
        {
            _serviceSchemes.Add(ErrorThemeScheme());
        }

        public static ThemeScheme ErrorThemeScheme()
        {
            return new ThemeScheme
            {
                Name = "Error",
                FontScheme = Aura.Schemes.FontsSchemes.ServiceFontsShemes.ErrorFontScheme(),
                ColorScheme = Aura.Schemes.ColorsSchemes.ServiceColorShemes.ErrorColorScheme(),
                IsServiceScheme = false
            };
        }
    }
}
