using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Connector.Models.Authorization;
using ConnectorCore.Models;
using Connector.ViewModels;

namespace Connector
{
    public class ConnectorApp : Notifier
    {
        public const string AppName = "Connector";
        public const string AppVersion = "A0";

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
        private ConnectorUser _currentUser;
        private UserSettings _connectorSettings;
        #endregion
        #region Properties
        public MainWindowViewModel WindowViewModel { get; set; }
        public ConnectorUser CurrentUser
        {
            get
            {
                return _currentUser;
            }
            set
            {
                _currentUser = value;
                OnPropertyChanged("CurrentUser");
            }
        }
        public UserSettings AppSettings
        {
            get
            {
                return _connectorSettings;
            }
            set
            {
                _connectorSettings = value;
                OnPropertyChanged("ConnectorSettings");
            }
        }
        #endregion
        #region Methods
        private void Initialize()
        {
            AppSettings = new UserSettings();
        }
        #endregion

    }
}
