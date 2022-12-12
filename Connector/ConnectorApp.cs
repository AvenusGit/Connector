using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows.Controls;
using System.Threading.Tasks;
using Connector.Models.Authorization;
using ConnectorCore.Models;
using Connector.ViewModels;
using Connector.Models;
using Connector.Models.REST;
using ConnectorCore.Models.Connections;
using Aura.VisualModels;
using System.Windows;
using Connector.View;
using AuraS.Controls.ControlsViewModels;
using AuraS.Controls;
using ConnectorCore.Models.VisualModels;
using Microsoft.VisualBasic.ApplicationServices;

namespace Connector
{
    public class ConnectorApp : Notifier
    {
        public const string AppName = "Connector";
        public static readonly ApplicationVersion AppVersion = new ApplicationVersion("A", 0, string.Empty);
        public string _connectorCenterUrl = "https://localhost:51980"; 

        #region Singletone
        private static ConnectorApp _connectorApp;
        private ConnectorApp()
        {
            Initialize();
            _timer = new Timer((UnitedSettings.JwtTokenLifeTimeMinutes * 60000));
            _timer.Elapsed += OnTokenTimerElapsed;
            _timer.AutoReset = false;
        }
        public static ConnectorApp Instance
        {
            get
            {
                if(_connectorApp is null)
                    _connectorApp = new ConnectorApp();
                return _connectorApp;
            }
        }
        #endregion
        #region Fields
        private bool _isTokenOld = true;
        private Timer _timer;
        private Session? _session;
        #endregion
        #region Properties
        public string ConnectorCenterUrl
        {
            get
            {
                return _connectorCenterUrl;
            }
            set
            {
                _connectorCenterUrl = value;
                OnPropertyChanged("ConnectorCenterUrl");
            }
        }
        public MainWindowViewModel WindowViewModel { get; set; }
        public Session? Session
        {
            get
            {
                if(_session is null)
                    _session = new Session();
                return _session;
            }
            set
            {
                _session = value;
                OnPropertyChanged("Session");
            }
        }
        public bool IsTokenOld 
        {
            get
            {
                return _isTokenOld;
            }
            set
            {
                if(!value)
                {
                    _timer.Start();
                }
                _isTokenOld = value;
            }
        }
        public WpfVisualScheme VisualScheme { get; set; } = new WpfVisualScheme()
        {
            ColorScheme = new WpfColorScheme().GetDefault(),
            FontScheme = new WpfFontScheme(string.Empty, 0).GetDefault()
        };
        public List<ConnectionWindow> ActiveConnections { get; set; } = new List<ConnectionWindow>();
        #endregion
        #region Methods
        private void Initialize()
        {
            Session = null;            
        }
        public async void Logout()
        {
            Session = null;
            IsTokenOld = true;
            foreach (RdpWindow rdp in ActiveConnections)
            {
                rdp.Close();
            }
            await WindowViewModel.ChangeUIControl(
                              new LoginControl(new LoginControllerViewModel(
                                  new Credentials(
                                      Session?.User?.Credentials.Login ?? string.Empty,
                                      string.Empty))),
                              true);
        }
        public void StartTokenTimer()
        {

            _timer.Start();
        }
        public async Task UpdateSessionConnectionsList()
        {
            if (Session is null)
                throw new Exception("Попытка обновления списка подключений без наличия сессии.");
            if (Session.User is null)
                throw new Exception("Попытка обновления списка подключений без наличия пользователя в сессии.");
            if (IsTokenOld)
                UpdateToken();
            RestService restService = new RestService();
            IEnumerable<Connection>? connections = await restService.GetConnectionListAsync();
            WindowViewModel.ShowBusyScreen("Обновление подключений...");
            if (connections is null)
                throw new Exception("Ошибка при авторизации. Не удалось десериализовать подключения.");

            Session.User.Connections = connections.ToList();
        }
        public async void UpdateToken()
        {
            try
            {
                if (Session is null || Session.User is null) return;
                WindowViewModel.ShowBusyScreen("Обновление токена...");
                RestService restService = new RestService();
                TokenInfo? tokenInfo = await restService.GetTokenInfoAsync(Session.User.Credentials);
                if (tokenInfo is null)
                    throw new Exception("Ошибка при авторизации. Не удалось десериализовать данные авторизации.");
                Session.Token = tokenInfo;
                IsTokenOld = false;
                WindowViewModel.HideBusyScreen();
            }
            catch (Exception ex)
            {
                AuraMessageWindow message = new AuraMessageWindow(
                    new AuraMessageWindowViewModel(
                        "Ошибка",
                        $"Ошибка при попытке обновить токен доступа. {ex.Message}",
                        "Ok",
                        AuraMessageWindowViewModel.MessageTypes.Error));
                message.ShowDialog();
                if(Session is null || Session.User is null)
                {
                    await WindowViewModel.ChangeUIControl(
                    new LoginControl(
                        new LoginControllerViewModel(new Credentials("",""))),
                    true);
                }
                else
                {
                    await WindowViewModel.ChangeUIControl(
                    new LoginControl(
                        new LoginControllerViewModel(Session.User.Credentials ?? new Credentials("", ""))),
                    true);
                }
                
                Session = null;
                WindowViewModel.HideBusyScreen();
            }
        }
        private void OnTokenTimerElapsed(object? sender, EventArgs e)
        {
            IsTokenOld = true;
        }
        #endregion

    }
}
