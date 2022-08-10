using Aura.Models;
using System.Windows;
using System.Windows.Markup;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Media;

namespace Aura.Helpers
{
    public class StyleHelper
    {
        /// <summary>
        /// Получение текущей схемы
        /// </summary>
        /// <returns></returns>
        public static StyleScheme GetCurrentStyleScheme()
        {
            string currentStyle = (string)Application.Current.Resources["CurrentStyleName"];
            return new StyleScheme
            {
                Name = currentStyle,
                ResourceDictionary = currentStyle,
                IsServiceScheme = false
            };
        }
    }
}
