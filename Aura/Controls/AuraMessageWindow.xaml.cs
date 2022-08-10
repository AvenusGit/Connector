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
using System.Windows.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Aura.Controls
{
    /// <summary>
    /// Логика взаимодействия для AuraMessageWindow.xaml
    /// </summary>
    public partial class AuraMessageWindow : Window
    {
        private string _logoText;
        private string _messageText;
        private string _buttonText;
        private MessageTypes _type;

        public AuraMessageWindow()
        {
            InitializeComponent();
        }

        public AuraMessageWindow(string logotype, string message, string buttontext, MessageTypes type)
        {
            InitializeComponent();
            Logotype = logotype;
            MessageType = type;
            Message = message;
            ButtonsText = buttontext;
        }

        public AuraMessageWindow( string message)
        {
            InitializeComponent();
            Logotype = "Информация";
            MessageType = MessageTypes.InformationType;
            Message = message;
            ButtonsText = "Ок";
        }

        public enum MessageTypes
        {
            InformationType,
            WarningType,
            ErrorType,
            JokeType
        }

        public string Logotype
        {
            get
            {
                return _logoText;
            }
            set
            {
                _logoText = value;
                Logo.Text = value;
            }
        }

        public string Message
        {
            get
            {
                return _messageText;
            }
            set
            {
                _messageText = value;
                Text.Text = value;
            }
        }

        public string ButtonsText
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

        public MessageTypes MessageType
        {
            get
            {
                return _type;
            }
            set
            {
                SetSelectedType(value);
                _type = value;
            }
        }

        private void SetSelectedType(MessageTypes selectedType)
        {
            switch (selectedType)
            {
                case MessageTypes.InformationType:
                    IconPath.Data = Schemes.Pathes.Pathes.GetPath(Schemes.Pathes.Pathes.InfoPath);
                    IconPath.Fill = (Brush)Application.Current.Resources["PathBrush"];
                    break;
                case MessageTypes.WarningType:
                    IconPath.Data = Schemes.Pathes.Pathes.GetPath(Schemes.Pathes.Pathes.WarningPath);
                    IconPath.Fill = Brushes.Yellow;
                    break;
                case MessageTypes.ErrorType:
                    IconPath.Data = Schemes.Pathes.Pathes.GetPath(Schemes.Pathes.Pathes.ErrorPath);
                    IconPath.Fill = (Brush)Application.Current.Resources["ErrorBrush"];
                    break;
                case MessageTypes.JokeType:
                    IconPath.Data = Schemes.Pathes.Pathes.GetPath(Schemes.Pathes.Pathes.JokePath);
                    IconPath.Fill = (Brush)Application.Current.Resources["PathBrush"];
                    break;
                default:
                    break;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();            
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
