using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Connector.ViewModels;
using System.Windows.Input;

namespace Connector.View
{
    /// <summary>
    /// Логика взаимодействия для LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        public LoginControl()
        {
            InitializeComponent();
            DataContext = new LoginControllerViewModel();
        }
        public LoginControl(LoginControllerViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            (DataContext as LoginControllerViewModel).Credentials.Password = iPasswordField.Password;
        }

        private void CheckEnterKey(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                (DataContext as LoginControllerViewModel).AuthorizeCommand.Execute(null!);
            }
        }
    }
}
