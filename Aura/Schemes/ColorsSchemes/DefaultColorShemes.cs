using Aura.Models;
using System.Windows;
using System.Windows.Markup;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Media;

namespace Aura.Schemes.ColorsSchemes
{
    public static class DefaultColorShemes
    {
        private static List<ColorScheme> _defaultSchemes = new List<ColorScheme>();

        public static List<ColorScheme> DefaultColorSchemes()
        {
            if (_defaultSchemes is null || _defaultSchemes.Count == 0)
            {
                InitializeDefaultColorScheme();
                return _defaultSchemes;
            }
            else
            {
                return _defaultSchemes;
            }
        }

        private static void InitializeDefaultColorScheme()
        {
            foreach (ThemeScheme theme in Aura.Schemes.ThemeSchemes.DefaultThemeShemes.DefaultThemeSchemes())
            {
                _defaultSchemes.Add(theme.ColorScheme);
            }
        }

        public static ColorScheme StandartColorScheme()
        {
            return new ColorScheme
            {
                Name = "По умолчанию",
                IsServiceScheme = false,

                FoneColor = Colors.AliceBlue,
                FoneBrush = new SolidColorBrush(Colors.AliceBlue),

                AccentColor = Colors.RoyalBlue,
                AccentBrush = new SolidColorBrush(Colors.RoyalBlue),

                SubAccentColor = (Color)ColorConverter.ConvertFromString("#FFBFCFFF"),
                SubAccentBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBFCFFF")),

                BorderColor = (Color)ColorConverter.ConvertFromString("#FFBFCFFF"),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBFCFFF")),

                PanelColor = Colors.AliceBlue,
                PanelBrush = new SolidColorBrush(Colors.AliceBlue),

                PathColor = Colors.RoyalBlue,
                PathBrush = new SolidColorBrush(Colors.RoyalBlue),

                TextColor = Colors.RoyalBlue,
                TextBrush = new SolidColorBrush(Colors.RoyalBlue),

                SelectionColor = Colors.Orange,
                SelectionBrush = new SolidColorBrush(Colors.Orange),

                ErrorColor = Colors.Red,
                ErrorBrush = new SolidColorBrush(Colors.Red),

                DisableColor = Colors.Gray,
                DisableBrush = new SolidColorBrush(Colors.Gray)
            };
        }

        public static ColorScheme DarkColorScheme()
        {
            return new ColorScheme
            {
                Name = "Темная",
                IsServiceScheme = false,

                FoneColor = (Color)ColorConverter.ConvertFromString("#FF000000"),
                FoneBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000")),

                AccentColor = Colors.White,
                AccentBrush = new SolidColorBrush(Colors.White),

                SubAccentColor = (Color)ColorConverter.ConvertFromString("#FF212121"),
                SubAccentBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF212121")),

                BorderColor = (Color)ColorConverter.ConvertFromString("#8D4F4F4F"),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8D4F4F4F")),

                PanelColor = (Color)ColorConverter.ConvertFromString("#FF121212"),
                PanelBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF121212")),

                PathColor = Colors.White,
                PathBrush = new SolidColorBrush(Colors.White),

                TextColor = Colors.White,
                TextBrush = new SolidColorBrush(Colors.White),

                SelectionColor = (Color)ColorConverter.ConvertFromString("#FF005A61"),
                SelectionBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF005A61")),

                ErrorColor = Colors.Red,
                ErrorBrush = new SolidColorBrush(Colors.Red),

                DisableColor = Colors.Gray,
                DisableBrush = new SolidColorBrush(Colors.Gray)
            };
        }

        public static ColorScheme OriginalColorScheme()
        {
            return new ColorScheme
            {
                Name = "Оригинальная",
                IsServiceScheme = false,

                FoneColor = (Color)ColorConverter.ConvertFromString("#FFFFF9DE"),
                FoneBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFF9DE")),

                AccentColor = (Color)ColorConverter.ConvertFromString("#FF404040"),
                AccentBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF404040")),

                SubAccentColor = (Color)ColorConverter.ConvertFromString("#FFC4C3B3"),
                SubAccentBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC4C3B3")),

                BorderColor = (Color)ColorConverter.ConvertFromString("#8D4F4F4F"),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8D4F4F4F")),

                PanelColor = (Color)ColorConverter.ConvertFromString("#FFDBDBC9"),
                PanelBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDBDBC9")),

                PathColor = (Color)ColorConverter.ConvertFromString("#FF333333"),
                PathBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF333333")),

                TextColor = (Color)ColorConverter.ConvertFromString("#FF333333"),
                TextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF333333")),

                SelectionColor = (Color)ColorConverter.ConvertFromString("#FFFF9F00"),
                SelectionBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF9F00")),

                ErrorColor = Colors.Red,
                ErrorBrush = new SolidColorBrush(Colors.Red),

                DisableColor = Colors.Gray,
                DisableBrush = new SolidColorBrush(Colors.Gray)
            };
        }

        public static ColorScheme SteelColorScheme()
        {
            return new ColorScheme
            {
                Name = "Сталь",
                IsServiceScheme = false,

                FoneColor = (Color)ColorConverter.ConvertFromString("#FFEBEBEB"),
                FoneBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEBEBEB")),

                AccentColor = (Color)ColorConverter.ConvertFromString("#FF474747"),
                AccentBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF474747")),

                SubAccentColor = (Color)ColorConverter.ConvertFromString("#FFA3A3A3"),
                SubAccentBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA3A3A3")),

                BorderColor = (Color)ColorConverter.ConvertFromString("#B6696969"),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B6696969")),

                PanelColor = (Color)ColorConverter.ConvertFromString("#FFFFFFFF"),
                PanelBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF")),

                PathColor = (Color)ColorConverter.ConvertFromString("#FF383838"),
                PathBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF383838")),

                TextColor = (Color)ColorConverter.ConvertFromString("#FF333333"),
                TextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF333333")),

                SelectionColor = (Color)ColorConverter.ConvertFromString("#FF005C7D"),
                SelectionBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF005C7D")),

                ErrorColor = Colors.Red,
                ErrorBrush = new SolidColorBrush(Colors.Red),

                DisableColor = Colors.Gray,
                DisableBrush = new SolidColorBrush(Colors.Gray)
            };
        }

        public static ColorScheme CustomColorScheme()
        {
            ColorScheme customScheme = Aura.Helpers.ColorHelper.GetCurrentColorScheme();
            customScheme.Name = "Настраиваемая";
            return customScheme;
        }

    }
}
