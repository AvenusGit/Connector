using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading.Tasks;
using Connector.Models.Authorization;
using ConnectorCore.Models;
using Connector.ViewModels;
using Connector.Models;
using Connector.Models.REST;
using ConnectorCore.Models.Connections;
using Aura.VisualModels;

namespace Connector
{
    public class ConnectorApp : Notifier
    {
        public const string AppName = "Connector";
        public static readonly ApplicationVersion AppVersion = new ApplicationVersion("A", 0, string.Empty);
        public string _connectorCenterUrl = "https://localhost:54411"; 

        #region Singletone
        private static ConnectorApp _connectorApp;
        private ConnectorApp()
        {
            Initialize();
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
        public WpfVisualScheme VisualScheme { get; set; }
        #endregion
        #region Methods
        private void Initialize()
        {
            Session = null;
        }

        public async Task UpdateSessionConnectionsList()
        {
            if (Session is null)
                throw new Exception("Попытка обновления списка подключений без наличия сессии.");
            if (Session.User is null)
                throw new Exception("Попытка обновления списка подключений без наличия пользователя в сессии.");
            RestService restService = new RestService();
            IEnumerable<Connection>? connections = await restService.GetConnectionListAsync();
            ConnectorApp.Instance.WindowViewModel.ShowBusyScreen("Обновление подключений...");
            if (connections is null)
                throw new Exception("Ошибка при авторизации. Не удалось десериализовать подключения.");

            Session.User.Connections = connections.ToList();
        }
        #endregion

    }
}
