using System;
using System.Windows;
using System.Windows.Markup;
using System.Globalization;
using System.Windows.Data;

namespace Aura.Converters
{
    public class ScaleToSizeConverter : IValueConverter
    {
        public double Size = 0;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Size * (double)Application.Current.Resources["Scale"];
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Size / (double)Application.Current.Resources["Scale"];
        }
    }
}
