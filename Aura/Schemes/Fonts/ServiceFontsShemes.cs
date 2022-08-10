using Aura.Models;
using System.Windows;
using System.Windows.Markup;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Media;

namespace Aura.Schemes.FontsSchemes
{
    public static class ServiceFontsShemes
    {
        private static List<FontScheme> _serviceSchemes = new List<FontScheme>();

        public static List<FontScheme> ServiceColorSchemes()
        {
            if (_serviceSchemes is null || _serviceSchemes.Count == 0)
            {
                InitializeServiceColorScheme();
                return _serviceSchemes;
            }
            else
            {
                return _serviceSchemes;
            }
        }

        private static void InitializeServiceColorScheme()
        {
            foreach (ThemeScheme theme in Aura.Schemes.ThemeSchemes.ServiceThemeShemes.ServiceThemeSchemes())
            {
                _serviceSchemes.Add(theme.FontScheme);
            }
        }

        public static FontScheme ErrorFontScheme()
        {
            return new FontScheme
            {
                Name = "Error",
                Font = new FontFamily("Arial"),
                Multiplier = 100,
                IsServiceScheme = false
            };
        }
    }
}
