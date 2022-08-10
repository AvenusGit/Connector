using Aura.Models;
using System.Windows;
using System.Windows.Markup;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Media;

namespace Aura.Helpers
{
    public class FontHelper
    {
        /// <summary>
        /// Поиск схемы по имени среди доступных
        /// </summary>
        /// <param name="name">Имя схемы</param>
        /// <param name="isServiceScheme">Служебная ли схема</param>
        /// <returns></returns>
        public static FontScheme GetUserSchemeByName( string name, bool isServiceScheme)
        {
            if(isServiceScheme)
                return Aura.Schemes.FontsSchemes.ServiceFontsShemes.ServiceColorSchemes().Find(x => x.Name == name);
            else
                return Aura.Schemes.FontsSchemes.DefaultFontsShemes.DefaultFontSchemes().Find(x => x.Name == name);
        }

        /// <summary>
        /// Получение текущей схемы
        /// </summary>
        /// <returns></returns>
        public static FontScheme GetCurrentFontScheme()
        {
            FontScheme result = new FontScheme();
            result.Name = "Loaded";
            result.Font = (FontFamily)Application.Current.Resources["Font"];
            result.Multiplier = (double)Application.Current.Resources["FontMultiplier"];
            result.IsServiceScheme = false;
            return result;
        }
    }
}
