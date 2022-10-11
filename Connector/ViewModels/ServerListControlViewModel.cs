﻿using System;
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
using Connector.Models.REST;
using Aura.VisualModels;
using System.Net;

namespace Connector.ViewModels
{
    public class ServerListControlViewModel : Notifier
    {
        #region Fields
        private bool _searchEnabled = false;
        private Command _updateConnectionsList;
        private Command _logoutCommand;
        private Command _toSettingsCommand;
        #endregion
        #region Properties
        public bool IsSearchEnabled
        {
            get { return _searchEnabled; }
            set
            {
                _searchEnabled = value;
                OnPropertyChanged("IsSearchEnabled");
            }
        }
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
        public ObservableCollection<Connection> Connections
        {
            get
            {
                if(CurrentUser is not null)
                    return new ObservableCollection<Connection>(CurrentUser.AllConnections);
                return new ObservableCollection<Connection>();
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
                      ConnectorApp.Instance.WindowViewModel.ShowBusyScreen("Обновление",true);
                      if (ConnectorApp.Instance.Session is null || ConnectorApp.Instance.Session.Token is null)
                          throw new Exception("Возникла ошибка при попытке обновить данные подключений. Сессия недоступна или отсутствует токен." +
                              "Необходима переавторизация.");
                      RestService restService = new RestService(ConnectorApp.Instance.Session!.Token!.Token);
                      IEnumerable<Connection>? updatedConnection = await restService.GetConnectionListAsync();
                      if (updatedConnection is not null)
                          ConnectorApp.Instance.Session.User!.Connections = updatedConnection.ToList();
                      else
                          throw new Exception("Возникла ошибка при попытке обновить данные подключений");

                      IEnumerable<AppUserGroup>? updatedGroups = await restService.GetGroupsListAsync();
                      if (updatedGroups is not null)
                          ConnectorApp.Instance.Session.User!.Groups = updatedGroups.ToList();
                      else
                          throw new Exception("Возникла ошибка при попытке обновить данные подключений");
                      OnPropertyChanged("CurrentUser");
                      ConnectorApp.Instance.WindowViewModel.HideBusyScreen();
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
                      ConnectorApp.Instance.Session = null;
                      await ConnectorApp.Instance.WindowViewModel.ChangeUIControl(
                          new LoginControl(new LoginControllerViewModel(
                              new Сredentials(
                                  ConnectorApp.Instance.Session?.User?.Credentials.Login ?? string.Empty,
                                  string.Empty))), 
                          true);
                      //ConnectorApp.Instance.VisualScheme.GetDefault().Apply();
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
