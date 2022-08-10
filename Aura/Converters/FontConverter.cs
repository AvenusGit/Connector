using System;
using System.Windows;
using System.Windows.Markup;
using System.Globalization;
using System.Windows.Data;

namespace Aura.Converters
{
    public class FontConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double oldValue;
            if((!double.TryParse((string)parameter, out oldValue)) || !(value is double))
              return null;
            double multiplier = (double)value;
            return Math.Round((multiplier / 100) * oldValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
