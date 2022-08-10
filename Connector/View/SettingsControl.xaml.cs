using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AuraS.Models;
using AuraS.Controls.ControlsViewModels;
using Connector.ViewModels;

namespace Connector.View
{
    /// <summary>
    /// Логика взаимодействия для SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();
            this.DataContext = new SettingsControlViewModel();
            iAuraSettingsControl.DataContext = 
                new AuraSettingControlViewModel(ConnectorApp.Instance.CurrentUser.VisualScheme);
        }
    }
}
