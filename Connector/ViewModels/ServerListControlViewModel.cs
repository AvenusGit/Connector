using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectorCore.Models;
using Connector.Models.Commands;
using ConnectorCore.Models.Connections;
using Connector.Models.Authorization;
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
        public AppUser? CurrentUser
        {
            get 
            {
                if(ConnectorApp.Instance.Session is not null)
                    if(ConnectorApp.Instance.Session.User is not null)
                        return ConnectorApp.Instance.Session.User; 
                return null;
            }
        }
        public ObservableCollection<Connection> RdpConnections
        {
            get
            {
                if(CurrentUser is not null)
                    return new ObservableCollection<Connection>(CurrentUser.Connections);
                return new ObservableCollection<Connection>();
            }
        }
        #endregion
        #region Commands
        //public Command UpdateConnectionListCommand
        //{
        //    get
        //    {
        //        return _updateConnectionsList ??
        //          (_updateConnectionsList = new Command(async obj =>
        //          {
        //              await ConnectorApp.Instance.CurrentUser.UpdateConnections(
        //                  new Action(() =>
        //                  {
        //                      ConnectorApp.Instance.WindowViewModel.ShowBusyScreen("Обновление подключений...");
        //                  }), 
        //                  new Action(() => ConnectorApp.Instance.WindowViewModel.HideBusyScreen()));
        //          }));
        //    }
        //}
        //public Command LogOutCommand
        //{
        //    get
        //    {
        //        return _logoutCommand ??
        //          (_logoutCommand = new Command(async obj =>
        //          {
        //              await ConnectorApp.Instance.WindowViewModel.ChangeUIControl(
        //                  new LoginControl(new LoginControllerViewModel(
        //                      new Сredentials(ConnectorApp.Instance.CurrentUser.Credentials.Login, string.Empty))),
        //                  true);
        //              ConnectorApp.Instance.CurrentUser = null!;
        //          }));
        //    }
        //}
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
