using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aura.Controls
{
    /// <summary>
    /// Логика взаимодействия для AuraMessage.xaml
    /// </summary>
    public partial class AuraMessage : UserControl
    {
        string _header = "";
        string _message = "";
        string _buttonText = "OK";
        MessageType _type = MessageType.Information;

        public AuraMessage()
        {
            InitializeComponent();
        }
        public AuraMessage(string message, MessageType type, string header = "", string buttonText = "")
        {
            InitializeComponent();
            _message = message;
            _header = header;
            _buttonText = buttonText;
        }

        public string Header
        {
            get
            {
                return _header;
            }
            set 
            {
                _header = value;
                MessageHeader.Text = value;
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                MessageText.Text = value;
            }
        }

        public string TextOnButton
        {
            get
            {
                return _buttonText;
            }
            set
            {
                _buttonText = value;
                ButtonText.Text = value;
            }
        }

        public MessageType TypeMessages
        {
            get 
            {
                return _type;
            }
            set
            {
                _type = value;
                switch (value)
                {
                    case MessageType.Error:
                        MessagePath.Data = Geometry.Parse(ErrorPath);
                        break;
                    case MessageType.Information:
                        MessagePath.Data = Geometry.Parse(InfoPath);
                        break;
                    case MessageType.Warning:
                        MessagePath.Data = Geometry.Parse(WarningPath);
                        break;
                    default:
                        break;
                }
            }
        }

        public void ShowMessage(string message, string header, string buttonText, MessageType type)
        {
            this.Message = message;
            Header = header;
            TextOnButton = buttonText;
            Visibility = Visibility.Visible;
        }

        public void ShowMessage(string message)
        {
            this.Message = message;
            Header = "";
            TextOnButton = "Ok";
            Visibility = Visibility.Visible;
        }

        public void ShowMessage()
        {
            Visibility = Visibility.Visible;
        }

        private static string ErrorPath = "M13,13H11V7H13M13,17H11V15H13M12," +
            " 2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10, 10 0 0,0 22,12A10,10 0 0,0 12,2Z";

        private static string WarningPath = "M13 14H11V9H13M13 18H11V16H13M1 21H23L12 2L1 21Z";

        private static string InfoPath = "M13.5,4A1.5,1.5 0 0,0 12,5.5A1.5,1.5 0 0,0 13.5,7A1.5," +
            "1.5 0 0,0 15,5.5A1.5,1.5 0 0,0 13.5,4M13.14,8.77C11.95,8.87 8.7,11.46 8.7,11.46C8.5," +
            "11.61 8.56,11.6 8.72,11.88C8.88,12.15 8.86,12.17 9.05,12.04C9.25,11.91 9.58,11.7 10.13," +
            "11.36C12.25,10 10.47,13.14 9.56,18.43C9.2,21.05 11.56,19.7 12.17,19.3C12.77,18.91 14.38," +
            "17.8 14.54,17.69C14.76,17.54 14.6,17.42 14.43,17.17C14.31,17 14.19,17.12 14.19,17.12C13.54," +
            "17.55 12.35,18.45 12.19,17.88C12,17.31 13.22,13.4 13.89,10.71C14,10.07 14.3,8.67 13.14,8.77Z";

        public enum MessageType
        { 
         Error,
         Information,
         Warning
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
