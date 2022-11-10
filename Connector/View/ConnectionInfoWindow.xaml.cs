using ConnectorCore.Models.Connections;
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
using Microsoft.Extensions;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MSTSCLib;
using AxMSTSCLib;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Windows.Forms.Integration;
using System.Drawing;
using ConnectorCore.Models;
using Microsoft.Extensions.Primitives;

namespace Connector.View
{
    /// <summary>
    /// Логика взаимодействия для RdpWindow.xaml
    /// </summary>
    public partial class ConnectionInfoWindow : Window
    {
        public ConnectionInfoWindow(Connection connection)
        {
            this.DataContext = this;
            Connection = connection;
            InitializeComponent();
        }
        public Connection Connection { get; set; }
        
        private void DragWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch { }
        }

        private void WindowClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
