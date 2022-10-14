using ConnectorCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Windows.Media;
using AuraS.Controls.ControlsViewModels;

namespace AuraS.Converters
{
    public class MessagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Geometry))
                throw new InvalidOperationException("The target must be a message type");

            switch (value)
            {
                
                case AuraMessageWindowViewModel.MessageTypes.Info:
                    return Geometry.Parse("M13.5,4A1.5,1.5 0 0,0 12,5.5A1.5,1.5 0 0,0 13.5,7A1.5,1.5 0 0,0 15,5.5A1.5,1.5 0 0,0 13.5,4M13.14,8.77C11.95,8.87 8.7,11.46 8.7,11.46C8.5,11.61 8.56,11.6 8.72,11.88C8.88,12.15 8.86,12.17 9.05,12.04C9.25,11.91 9.58,11.7 10.13,11.36C12.25,10 10.47,13.14 9.56,18.43C9.2,21.05 11.56,19.7 12.17,19.3C12.77,18.91 14.38,17.8 14.54,17.69C14.76,17.54 14.6,17.42 14.43,17.17C14.31,17 14.19,17.12 14.19,17.12C13.54,17.55 12.35,18.45 12.19,17.88C12,17.31 13.22,13.4 13.89,10.71C14,10.07 14.3,8.67 13.14,8.77Z");
                case AuraMessageWindowViewModel.MessageTypes.Warning:
                    return Geometry.Parse("M11,15H13V17H11V15M11,7H13V13H11V7M12,2C6.47,2 2,6.5 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M12,20A8,8 0 0,1 4,12A8,8 0 0,1 12,4A8,8 0 0,1 20,12A8,8 0 0,1 12,20Z");
                case AuraMessageWindowViewModel.MessageTypes.Error:
                    return Geometry.Parse("M15.8,18.5L21.8,20.1L21.4,22L12,19.5L2.6,22L2.1,20.1L8.1,18.5L2,16.9L2.5,15L11.9,17.5L21.3,15L21.8,16.9L15.8,18.5M9.5,6C8.7,6 8,6.7 8,7.5C8,8.3 8.7,9 9.5,9C10.3,9 11,8.3 11,7.5C11,6.7 10.3,6 9.5,6M14.5,6C13.7,6 13,6.7 13,7.5C13,8.3 13.7,9 14.5,9C15.3,9 16,8.3 16,7.5C16,6.7 15.3,6 14.5,6M13,11L12,9L11,11H13M12,1C8.1,1 5,4.1 5,8C5,9.9 5.8,11.6 7,12.9V16H17V12.9C18.2,11.6 19,9.9 19,8C19,4.1 15.9,1 12,1M15,12V14H14V12H13V14H11V12H10V14H9V12H9C7.8,11.1 7,9.7 7,8C7,5.2 9.2,3 12,3C14.8,3 17,5.2 17,8C17,9.6 16.2,11.1 15,12Z");
                default: throw new Exception("Unknow message type");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
