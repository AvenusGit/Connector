using ConnectorCore.Models.Connections;
using System;
using System.Windows;
using MSTSCLib;
using AxMSTSCLib;
using System.ComponentModel;
using ConnectorCore.Models;
using Connector.Models;

namespace Connector.View
{
    public partial class RdpWindow : ConnectionWindow
    {
        public RdpWindow(Connection connection)
        {
            this.DataContext = this;
            Connection = connection;
            WindowLogo = $"{Connection.Server.Name}:{Connection.ConnectionName}";
            InitializeComponent();
            Loaded += Connect;
        }
        public AxMsRdpClient10NotSafeForScripting? Client { get; set; }
        public override void Connect(object? sender, EventArgs e)
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
            Client.Name = WindowLogo;
            iWindowsFormsHost.Child = Client;

            if(ConnectorApp.Instance.Session is null || ConnectorApp.Instance.Session.User is null)
            {
                OnClose(this, null);
                AuraS.Controls.AuraMessageWindow message = new AuraS.Controls.AuraMessageWindow(new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel
                    ("Ошибка при подключении", "Отсутствует сессия пользователя. Переавторизауйтесь в приложении.", "Ок",
                    AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Error));
                ConnectorApp.Instance.Logout();
                return;
            }
            if (ConnectorApp.Instance.Session.User.UserSettings is null)
            {
                OnClose(this, null);
                AuraS.Controls.AuraMessageWindow message = new AuraS.Controls.AuraMessageWindow(new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel
                    ("Ошибка при подключении", "Отсутствуют настройки пользователя. Они будут назначены по умолчанию.", "Ок",
                    AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Warning));
                ConnectorApp.Instance.Session.User.UserSettings = UserSettings.GetDefault();
            }

            //visual settings
            Client.ColorDepth = ConnectorApp.Instance.Session.User.UserSettings.RdpSettings.ColorDepth;        
            Client.Width = Client.Parent.ClientRectangle.Width; //Convert.ToInt32(iHost.ActualWidth);
            Client.Height = Client.Parent.ClientRectangle.Height;//Convert.ToInt32(iHost.ActualHeight);
            Client.AdvancedSettings9.SmartSizing = ConnectorApp.Instance.Session.User.UserSettings.RdpSettings.SmartSizing;
            Client.DesktopWidth = Convert.ToInt32(SystemParameters.FullPrimaryScreenWidth);
            Client.DesktopHeight = Convert.ToInt32(SystemParameters.FullPrimaryScreenHeight);
            Client.AdvancedSettings9.AuthenticationLevel = 2;
            Client.AdvancedSettings9.EnableCredSspSupport = true;
            Client.FullScreen = ConnectorApp.Instance.Session.User.UserSettings.RdpSettings.AutoFullScreen;
            Client.FullScreenTitle = WindowLogo;
            Client.AdvancedSettings.ContainerHandledFullScreen = 0;
            Client.AdvancedSettings8.RelativeMouseMode = true;
            Client.AdvancedSettings.BitmapPeristence = 1;
            Client.AdvancedSettings.Compress = 1;
            //Client.AdvancedSettings2.overallConnectionTimeout = 1;//?
            Client.AdvancedSettings2.DisableCtrlAltDel = ConnectorApp.Instance.Session.User.UserSettings.RdpSettings.DisableCtrlAltDel ? 1 : 0;
            Client.AdvancedSettings2.RedirectDrives = ConnectorApp.Instance.Session.User.UserSettings.RdpSettings.RedirectDrives;
            Client.AdvancedSettings2.RedirectPrinters = ConnectorApp.Instance.Session.User.UserSettings.RdpSettings.RedirectPrinters;
            Client.AdvancedSettings2.RedirectSmartCards = ConnectorApp.Instance.Session.User.UserSettings.RdpSettings.RedirectSmartCards;
            Client.AdvancedSettings9.EnableAutoReconnect = ConnectorApp.Instance.Session.User.UserSettings.RdpSettings.EnableAutoReconnect;
            Client.AdvancedSettings9.RedirectDirectX = ConnectorApp.Instance.Session.User.UserSettings.RdpSettings.RedirectDirectX;

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
            Client.OnDisconnected += OnDisconnected;
            Client.OnIdleTimeoutNotification += OnTimeout;


            //connection settings
            Client.Name = $"Подключение к {Connection.ConnectionName}";
            Client.Server = Connection.Server.Host;
            Client.UserName = Connection.ServerUser.Credentials.Login;
            Client.AdvancedSettings9.ClearTextPassword = Connection.ServerUser.Credentials.Password ?? "";
            Client.AdvancedSettings9.RDPPort = Connection.Server.RdpPort;

            ((ISupportInitialize)Client).EndInit();

            try
            {
                Client.Connect();
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
            Client.RequestClose();
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
        }
        private void OnWindowMode(object? sender, EventArgs e)
        {
            Client!.FullScreen = false;
            Client.Show();
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
        private void OnDisconnected(object? sender, IMsTscAxEvents_OnDisconnectedEvent e)
        {
            OnClosed(null);
            this.Close();
        }
        private void OnTimeout(object? sender, EventArgs e)
        {
            AuraS.Controls.AuraMessageWindow messageWindow = new AuraS.Controls.AuraMessageWindow(
                    new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel(
                        "Информация",
                        $"Не удалось подключиться к {Connection.Server.Name}.",
                        "Ok",
                        AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Info)
                );
        }
        #endregion

        private void Maximized(object sender, RoutedEventArgs e)
        {
            OnFullScreenMode(sender, e);
        }
        private void Minimize(object sender, RoutedEventArgs e)
        {
            OnMinimize(sender, e);
        }
        private void RdpWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Client is null) return;
            if(Client.FullScreen)
            {
                Client.Width = Convert.ToInt32(SystemParameters.FullPrimaryScreenWidth);
                Client.Height = Convert.ToInt32(SystemParameters.FullPrimaryScreenHeight);
            }
            else
            {
                Client.Height = Convert.ToInt32(iHost.ActualHeight);
                Client.Width = Convert.ToInt32(iHost.ActualWidth);               
            }            
        }
        public void WindowClosing(object sender, CancelEventArgs e)
        {
            if (Client is not null)
                if (Client.Connected == 2)
                    Client.Disconnect();
            ConnectorApp.Instance.ActiveConnections.Remove(this);
        }
    }
}
