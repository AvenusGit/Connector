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
            Client.OnLoginComplete += OnLoginComplete;


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
            catch (Exception ex)
            {
                OnClose(this, null);
                AuraS.Controls.AuraMessageWindow message = new AuraS.Controls.AuraMessageWindow(new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel
                    ($"Не удалось подключиться через {Connection.ConnectionName}", ex.Message, "Ок",
                    AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Error));
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
            Client?.RequestClose();
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
        private void OnLoginComplete (object? sender, EventArgs e)
        {
            BusyMessage = "Авторизация пройдена...";
        }
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
            // codes: https://learn.microsoft.com/en-us/windows/win32/termserv/imstscaxevents-ondisconnected
            switch (e.discReason)
            {
                case 2308:
                    AuraS.Controls.AuraMessageWindow message =
                    new AuraS.Controls.AuraMessageWindow(new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel
                    ($"Ошибка подключения к {Connection.ConnectionName}", "Сокет был неожиданно закрыт", "Ок",
                    AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Error));
                    message.ShowDialog();
                    break;
                case 3:
                case 2:
                    message =
                    new AuraS.Controls.AuraMessageWindow(new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel
                    ($"Закрытие подключения к {Connection.ConnectionName}", "Ваш сеанс был завершен удаленно", "Ок",
                    AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Error));
                    message.ShowDialog();
                    break;
                case 776:
                    message =
                    new AuraS.Controls.AuraMessageWindow(new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel
                    ($"Ошибка подключения к {Connection.ConnectionName}", "Недопустимый IP адрес", "Ок",
                    AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Error));
                    message.ShowDialog();
                    break;
                case 2823:
                    message =
                    new AuraS.Controls.AuraMessageWindow(new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel
                    ($"Ошибка подключения к {Connection.ConnectionName}", "Учетная запись отключена, обратитесь к администратору", "Ок",
                    AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Error));
                    message.ShowDialog();
                    break;
                case 3591:
                    message =
                    new AuraS.Controls.AuraMessageWindow(new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel
                    ($"Ошибка подключения к {Connection.ConnectionName}", "Срок действия учетная записи истек, обратитесь к администратору", "Ок",
                    AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Error));
                    message.ShowDialog();
                    break;
                case 2567:
                    message =
                    new AuraS.Controls.AuraMessageWindow(new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel
                    ($"Ошибка подключения к {Connection.ConnectionName}", "Пользователь не имеет учетной записи, обратитесь к администратору", "Ок",
                    AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Error));
                    message.ShowDialog();
                    break;
                case 2055:
                    message =
                    new AuraS.Controls.AuraMessageWindow(new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel
                    ($"Ошибка подключения к {Connection.ConnectionName}", "Не удалось авторизоваться на сервере, обратитесь к администратору", "Ок",
                    AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Error));
                    message.ShowDialog();
                    break;
                case 262:
                case 518:
                case 774:
                    message =
                    new AuraS.Controls.AuraMessageWindow(new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel
                    ($"Ошибка подключения к {Connection.ConnectionName}", "Нехватает памяти, обратитесь к администратору", "Ок",
                    AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Error));
                    message.ShowDialog();
                    break;
                case 2056:
                    message =
                    new AuraS.Controls.AuraMessageWindow(new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel
                    ($"Ошибка подключения к {Connection.ConnectionName}", "Ошибка RDP лицензии, обратитесь к администратору", "Ок",
                    AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Error));
                    message.ShowDialog();
                    break;
                case 3078:
                case 2822:
                case 2310:
                case 2566:
                case 1286:
                case 1542:
                case 1030:
                case 1798:
                case 6919:
                case 8455:
                case 6151:
                case 5895:
                    message =
                    new AuraS.Controls.AuraMessageWindow(new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel
                    ($"Ошибка подключения к {Connection.ConnectionName}", $"Ошибка безопасности, обратитесь к администратору ({e.discReason})", "Ок",
                    AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Error));
                    message.ShowDialog();
                    break;
                case 3847:
                    message =
                    new AuraS.Controls.AuraMessageWindow(new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel
                    ($"Ошибка подключения к {Connection.ConnectionName}", "Срок действия пароля истек, обратитесь к администратору", "Ок",
                    AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Error));
                    message.ShowDialog();
                    break;
                case 4615:
                    message =
                    new AuraS.Controls.AuraMessageWindow(new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel
                    ($"Ошибка подключения к {Connection.ConnectionName}", "Необходимо изменить пароль перед первым входом, обратитесь к администратору", "Ок",
                    AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Error));
                    message.ShowDialog();
                    break;
                case 3335:
                case 3079:
                    message =
                    new AuraS.Controls.AuraMessageWindow(new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel
                    ($"Ошибка подключения к {Connection.ConnectionName}", "Учетная запись заблокирована или ограничена, обратитесь к администратору", "Ок",
                    AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Error));
                    message.ShowDialog();
                    break;

                case 264:
                case 260:
                case 1288:
                case 1540:
                case 520:
                case 2312:
                case 516:
                case 1028:
                case 1796:
                    message =
                    new AuraS.Controls.AuraMessageWindow(new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel
                    ($"Ошибка подключения к {Connection.ConnectionName}", $"Истекло время ожидания сервера или сервер отказал в подключении ({e.discReason})", "Ок",
                    AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Error));
                    message.ShowDialog();
                    break;
                case 1:
                    // normal close connection
                    break;
                default:
                    message =
                    new AuraS.Controls.AuraMessageWindow(new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel
                    ($"Ошибка подключения к {Connection.ConnectionName}", $"По неизвестной причине инициировано закрытие подключения ({e.discReason}). Обратитесь к администратору.", "Ок",
                    AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Error));
                    message.ShowDialog();
                    break;
            }
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
