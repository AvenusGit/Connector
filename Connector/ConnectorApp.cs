using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Connector.Models.Authorization;
using ConnectorCore.Models;
using Connector.ViewModels;
using Connector.Models;

namespace Connector
{
    public class ConnectorApp : Notifier
    {
        public const string AppName = "Connector";
        public static readonly ApplicationVersion AppVersion = new ApplicationVersion("A", 0, string.Empty);
        public const string ConnectorCenterUrl = "https://localhost:51531"; 

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
        #endregion
        #region Methods
        private void Initialize()
        {
            Session = null;
        }
        #endregion

    }
}
