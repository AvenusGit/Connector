using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AuraS.Controls.ControlsViewModels;

namespace AuraS.Controls
{
    /// <summary>
    /// Логика взаимодействия для AuraMessageWindow.xaml
    /// </summary>
    public partial class AuraMessageWindow : Window
    {
        public AuraMessageWindow(AuraMessageWindowViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
        private void DragWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch { }
        }
        private void CloseMessageWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
