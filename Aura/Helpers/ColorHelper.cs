using Aura.Models;
using System.Windows;
using System.Windows.Markup;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Media;

namespace Aura.Helpers
{
    public static class ColorHelper
    {     
        /// <summary>
        /// Поиск схемы по имени среди доступных
        /// </summary>
        /// <param name="name">Имя схемы</param>
        /// <param name="isServiceScheme">Сервисная схема</param>
        /// <returns></returns>
        public static ColorScheme GetUserSchemeByName(string name, bool isServiceScheme)
        {
            if(isServiceScheme)
                return Aura.Schemes.ColorsSchemes.ServiceColorShemes.ServiceColorSchemes().Find(x => x.Name == name);
            else
                return Aura.Schemes.ColorsSchemes.DefaultColorShemes.DefaultColorSchemes().Find(x => x.Name == name);
        }
        /// <summary>
        /// Получение текущей схемы
        /// </summary>
        /// <returns></returns>
        public static ColorScheme GetCurrentColorScheme()
        {
            ColorScheme result = new ColorScheme();
            result.Name = "CurrentTheme";
            result.IsServiceScheme = false;
            result.FoneColor = (Color)Application.Current.Resources["CFone"];
            result.FoneBrush = (SolidColorBrush)Application.Current.Resources["FoneBrush"];

            result.AccentColor = (Color)Application.Current.Resources["CAccent"];
            result.AccentBrush = (SolidColorBrush)Application.Current.Resources["AccentBrush"];

            result.SubAccentColor = (Color)Application.Current.Resources["CSubAccent"];
            result.SubAccentBrush = (SolidColorBrush)Application.Current.Resources["SubAccentBrush"];

            result.BorderColor = (Color)Application.Current.Resources["CBorder"];
            result.BorderBrush = (SolidColorBrush)Application.Current.Resources["BorderBrush"];

            result.PanelColor = (Color)Application.Current.Resources["CPanel"];
            result.PanelBrush = (SolidColorBrush)Application.Current.Resources["PanelBrush"];

            result.PathColor = (Color)Application.Current.Resources["CPath"];
            result.PathBrush = (SolidColorBrush)Application.Current.Resources["PathBrush"];

            result.TextColor = (Color)Application.Current.Resources["CText"];
            result.TextBrush = (SolidColorBrush)Application.Current.Resources["TextBrush"];

            result.SelectionColor = (Color)Application.Current.Resources["CSelect"];
            result.SelectionBrush = (SolidColorBrush)Application.Current.Resources["SelectBrush"];

            result.ErrorColor = (Color)Application.Current.Resources["CError"];
            result.ErrorBrush = (SolidColorBrush)Application.Current.Resources["ErrorBrush"];

            result.DisableColor = (Color)Application.Current.Resources["CDisable"];
            result.DisableBrush = (SolidColorBrush)Application.Current.Resources["DisableBrush"];
            return result;
        }
    }
}
