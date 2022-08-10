using Aura.Models;
using System.Windows;
using System.Windows.Markup;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Media;

namespace Aura.Schemes.ColorsSchemes
{
    public class ServiceColorShemes
    {
        private static List<ColorScheme> _serviceColorSchemes = new List<ColorScheme>();

        public static List<ColorScheme> ServiceColorSchemes()
        {
            if (_serviceColorSchemes is null || _serviceColorSchemes.Count == 0)
            {
                InitializeServiceColorScheme();
                return _serviceColorSchemes;
            }
            else
            {
                return _serviceColorSchemes;
            }
        }

        private static void InitializeServiceColorScheme()
        {
            foreach (ThemeScheme theme in Aura.Schemes.ThemeSchemes.ServiceThemeShemes.ServiceThemeSchemes())
            {
                _serviceColorSchemes.Add(theme.ColorScheme);
            }
        }

        public static ColorScheme ErrorColorScheme()
        {
            return new ColorScheme
            {
                Name = "Ошибка",
                IsServiceScheme = true,

                FoneColor = (Color)ColorConverter.ConvertFromString("#FFC20000"),
                FoneBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC20000")),

                AccentColor = (Color)ColorConverter.ConvertFromString("#FFFFFFFF"),
                AccentBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF")),

                SubAccentColor = (Color)ColorConverter.ConvertFromString("#FF870000"),
                SubAccentBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF870000")),

                BorderColor = (Color)ColorConverter.ConvertFromString("#FFA10000"),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA10000")),

                PanelColor = (Color)ColorConverter.ConvertFromString("#FF910000"),
                PanelBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF910000")),

                PathColor = (Color)ColorConverter.ConvertFromString("#FFFFFFFF"),
                PathBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF")),

                TextColor = (Color)ColorConverter.ConvertFromString("#FFFFFFFF"),
                TextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF")),

                SelectionColor = Colors.Orange,
                SelectionBrush = new SolidColorBrush(Colors.Orange),

                ErrorColor = Colors.Red,
                ErrorBrush = new SolidColorBrush(Colors.Red),

                DisableColor = Colors.Gray,
                DisableBrush = new SolidColorBrush(Colors.Gray)
            };
        }
    }
}
