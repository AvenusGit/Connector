using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Connector.ViewModels;

namespace Connector.View
{
    /// <summary>
    /// Логика взаимодействия для AppSettingsControl.xaml
    /// </summary>
    public partial class AppSettingsControl : UserControl
    {
        UserControl _backControl;
        public AppSettingsControl(UserControl backControl)
        {
            _backControl = backControl;
            InitializeComponent();
            DataContext = ConnectorApp.Instance;
        }

        private async void OnSave(object sender, RoutedEventArgs e)
        {
            await ConnectorApp.Instance.WindowViewModel.ChangeUIControl(_backControl, true);
        }
    }
}
