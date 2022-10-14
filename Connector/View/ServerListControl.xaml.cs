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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Connector.ViewModels;
using ConnectorCore.Models.Connections;

namespace Connector.View
{
    /// <summary>
    /// Логика взаимодействия для ServerListControl.xaml
    /// </summary>
    public partial class ServerListControl : UserControl
    {
        public ServerListControl(ServerListControlViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void SearchCancel(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrWhiteSpace(iSearch.Text))
            {
                (DataContext as ServerListControlViewModel).IsSearchEnabled = false;
            }
            else
                iSearch.Text = String.Empty;

        }
        private void SearchPanelOpen(object sender, RoutedEventArgs e)
        {
            (DataContext as ServerListControlViewModel).IsSearchEnabled = true;
        }

        private void FilterTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(iSearch.Text))
            {
                if (iConnectionsListBox.ItemsSource is IEnumerable<Connection>)
                {
                    Predicate<object> filter = Filter;
                    iConnectionsListBox.Items.Filter = filter;
                }                
            }
            else
                iConnectionsListBox.Items.Filter = null;
        }

        private bool Filter(object connection)
        {
            try
            {
                if (connection is Connection)
                {
                    string filterText = iSearch.Text.ToLower();
                    if (((Connection)connection)
                        .ConnectionName
                        .ToLower()
                        .Contains(filterText) ||
                        ((Connection)connection).ConnectionType
                        .ToString()
                        .ToLower()
                        .Contains(filterText) ||
                        ((Connection)connection).Server.Name
                        .ToLower()
                        .Contains(filterText) ||
                        ((Connection)connection).ServerUser.Name
                        .ToLower()
                        .Contains(filterText))
                            return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
