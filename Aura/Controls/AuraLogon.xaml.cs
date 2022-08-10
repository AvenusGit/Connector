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

namespace Aura.Controls
{
    /// <summary>
    /// Логика взаимодействия для AuraLogon.xaml
    /// </summary>
    public partial class AuraLogon : Window
    {
        public AuraLogon()
        {
            InitializeComponent();
        }

        private void enter_Click(object sender, RoutedEventArgs e)
        {
            Messenger.ShowMessage("Это стенд. Тут никуда не войдешь))", "Ошибка", "Блин", AuraMessage.MessageType.Error);
        }

        private void escape_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void closebutton_Click(object sender, RoutedEventArgs e)
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
