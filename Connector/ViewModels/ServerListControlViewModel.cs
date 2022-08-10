using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectorCore.Models.Server;
using ConnectorCore.Models.Authorization;
using Connector.Models.Commands;
using Connector.Models.Authorization;
using ConnectorCore.Interfaces;
using Connector.View;

namespace Connector.ViewModels
{
    public class ServerListControlViewModel : Notifier
    {
        #region Fields
        private Command _updateConnectionsList;
        private Command _logoutCommand;
        private Command _toSettingsCommand;
        #endregion
        #region Properties
        public ConnectorUser CurrentUser
        {
            get { return ConnectorApp.Instance.CurrentUser; }
        }
        public ObservableCollection<IConnection> RdpConnections
        {
            get
            {
                return new ObservableCollection<IConnection>(CurrentUser.Connections);
            }
        }
        #endregion
        #region Commands
        public Command UpdateConnectionListCommand
        {
            get
            {
                return _updateConnectionsList ??
                  (_updateConnectionsList = new Command(async obj =>
                  {
                      await ConnectorApp.Instance.CurrentUser.UpdateConnections(
                          new Action(() =>
                          {
                              ConnectorApp.Instance.WindowViewModel.ShowBusyScreen("Обновление подключений...");
                          }), 
                          new Action(() => ConnectorApp.Instance.WindowViewModel.HideBusyScreen()));
                  }));
            }
        }
        public Command LogOutCommand
        {
            get
            {
                return _logoutCommand ??
                  (_logoutCommand = new Command(async obj =>
                  {
                      await ConnectorApp.Instance.WindowViewModel.ChangeUIControl(
                          new LoginControl(new LoginControllerViewModel(
                              new Сredentials(ConnectorApp.Instance.CurrentUser.Credentials.Login, string.Empty))),
                          true);
                      ConnectorApp.Instance.CurrentUser = null!;
                  }));
            }
        }
        public Command ToSettingsCommand
        {
            get
            {
                return _toSettingsCommand ??
                    (_toSettingsCommand = new Command(async obj =>
                    {
                        await ConnectorApp.Instance.WindowViewModel.ChangeUIControl(new SettingsControl(), true);
                    }));
            }
        }
        #endregion

    }
}
