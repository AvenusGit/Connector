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

namespace Connector.View
{
    /// <summary>
    /// Логика взаимодействия для RdpWindow.xaml
    /// </summary>
    public partial class RdpWindow : Window, INotifyPropertyChanged
    {
        public RdpWindow(Connection connection)
        {
            this.DataContext = this;
            Connection = connection;
            WindowLogo = $"{Connection.Server.Name}:{Connection.ConnectionName}";
            InitializeComponent();            
        }
        private bool _isBusy;
        private string _busyMessage;
        public bool IsBusy 
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }
        public string BusyMessage 
        {
            get
            {
                return _busyMessage;
            }
            set
            {
                _busyMessage = value;
                OnPropertyChanged("BusyMessage");
            }
        } 
        public string WindowLogo { get; set; }
        public Connection Connection { get; set; }
        public AxMsRdpClient10NotSafeForScripting? Client { get; set; }
        public void Connect()
        {
            BusyMessage = "Идет подключение...";
            IsBusy = true;

            if (Connection is null)
                throw new Exception("Не удалось подключиться к RDP серверу. Подключение не определено. Обратитесь к администратору.");
            if (Connection.ServerUser.Credentials is null)
                throw new Exception("Не удалось подключиться к RDP серверу. Не указаны данные авторизации. Обратитесь к администратору.");
            if (String.IsNullOrEmpty(Connection.ServerUser.Credentials.Login))
                throw new Exception("Не удалось подключиться к RDP серверу. Не указан логин пользователя. Обратитесь к администратору.");
            if (String.IsNullOrEmpty(Connection.Server.Host))
                throw new Exception("Не удалось подключиться к RDP серверу. Не указан хост. Обратитесь к администратору.");

            Client = new AxMsRdpClient10NotSafeForScripting();   
            ((ISupportInitialize)Client).BeginInit();
            iWindowsFormsHost.Child = Client;
            
            //events
            Client.ConnectingText = "Идет подключение...";
            Client.DisconnectedText = "Идет отключение...";
            Client.OnConnecting += OnConnection;
            Client.OnConnected += OnConnected;
            Client.OnAutoReconnecting += OnReconnection;
            Client.OnAutoReconnected += OnAutoReconnected;
            Client.OnConfirmClose += OnClose;
            Client.OnConnectionBarPullDown += PinBarDown;
            Client.OnEnterFullScreenMode += OnFullScreenMode;
            Client.OnLeaveFullScreenMode += OnWindowMode;
            Client.OnRequestContainerMinimize += OnMinimize;
            Client.OnFatalError += OnFatalError;
            Client.OnWarning += OnNoFatalError;
            ((ISupportInitialize)Client).EndInit();            

            //connection settings
            Client.Name = $"Подключение к {Connection.ConnectionName}";
            Client.Server = Connection.Server.Host;
            Client.UserName = Connection.ServerUser.Credentials.Login;
            Client.AdvancedSettings2.ClearTextPassword = Connection.ServerUser.Credentials.Password ?? "";
            Client.AdvancedSettings2.RDPPort = Connection.Server.RdpPort;

            //visual settings
            Client.ColorDepth = 24;
            Client.AdvancedSettings9.SmartSizing = true;
            Client.Width = Convert.ToInt32(iHost.ActualWidth);
            Client.Height = Convert.ToInt32(iHost.ActualHeight);
            Client.DesktopWidth = Convert.ToInt32(iHost.ActualWidth);
            Client.DesktopHeight = Convert.ToInt32(iHost.ActualHeight);
            Client.AdvancedSettings9.AuthenticationLevel = 2;
            Client.AdvancedSettings9.EnableCredSspSupport = true;
            Client.FullScreen = true;
            Client.FullScreenTitle = WindowLogo;
            Client.AdvancedSettings.ContainerHandledFullScreen = 0;

            try
            {
                Client.Connect();
                Client.Show();
            }
            catch
            {
            }            
        }
        #region Connecting
        private void OnConnection(object? sender, EventArgs e)
        {
            IsBusy = true;
            BusyMessage = "Идет подключение...";
        }
        private void OnConnected(object? sender, EventArgs e)
        {
            IsBusy = false;
        }
        #endregion
        #region Reconnecting
        private AutoReconnectContinueState OnReconnection(object sender, IMsTscAxEvents_OnAutoReconnectingEvent e)
        {
            IsBusy = true;
            BusyMessage = $"Идет переподключение...({e.attemptCount})";
            return AutoReconnectContinueState.autoReconnectContinueAutomatic;
        }
        private void OnAutoReconnected(object? sender, EventArgs e)
        {
            IsBusy = false;
        }
        #endregion
        #region Close connection/Window
        private bool OnClose(object sender, IMsTscAxEvents_OnConfirmCloseEvent e)
        {
            this.Close();
            return true; // TODO realise confirm
        }
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            OnClose(sender, new IMsTscAxEvents_OnConfirmCloseEvent());
            this.Close();
        }
        #endregion
        #region FullScreen
        private void OnFullScreenMode(object? sender, EventArgs e)
        {
            Client!.FullScreen = true;
            this.WindowState = WindowState.Minimized;
        }
        private void OnWindowMode(object? sender, EventArgs e)
        {
            Client!.FullScreen = false;
            this.WindowState = WindowState.Normal;
        }
        #endregion
        #region Minimize
        private void OnMinimize(object? sender, EventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        #endregion
        #region Other
        private void PinBarDown(object? sender, EventArgs e)
        {
            OnWindowMode(sender, e);
        }
        private void OnNoFatalError(object? sender, IMsTscAxEvents_OnWarningEvent e)
        {
            AuraS.Controls.AuraMessageWindow messageWindow = new AuraS.Controls.AuraMessageWindow(
                    new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel(
                        "Предупреждение",
                        "Код предупреждения:" + e.warningCode,
                        "Ok",
                        AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Warning)
                );
        }
        private void OnFatalError(object? sender, IMsTscAxEvents_OnFatalErrorEvent e)
        {
            Client!.RequestClose();
            this.Close();
            throw new Exception("Ошибка в RDP подлючении:" + e.errorCode);
        }
        #endregion
        private void DragWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch { }
        }
        private void Maximized(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;              
            else
                OnFullScreenMode(sender, e);
        }
        private void Minimize(object sender, RoutedEventArgs e)
        {
            OnMinimize(sender, e);
        }

        #region InotifyPropertyChange
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}
