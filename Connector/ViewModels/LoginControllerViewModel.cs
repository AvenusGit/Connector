using System;
using Connector.Models.Commands;
using Connector.Models.REST;
using Connector.Models.Authorization;
using Connector.View;
using ConnectorCore.Models;
using Aura.VisualModels;
using AuraS.Controls;
using AuraS.Controls.ControlsViewModels;
using System.Windows;
using System.Collections.Generic;
using ConnectorCore.Models.Connections;
using System.Linq;

namespace Connector.ViewModels
{
    public class LoginControllerViewModel : Notifier
    {
        #region Constructors
        public LoginControllerViewModel() 
        {
            Сredentials = new Сredentials(string.Empty, string.Empty);
        }
        public LoginControllerViewModel(Сredentials credentials)
        {
            Сredentials = credentials;
        }
        #endregion
        #region Fields
        private Command _authorizeCommand;
        #endregion
        #region Properties
        public string AppName
        {
            get
            {
                return ConnectorApp.AppName;
            }
        }
        public Сredentials Сredentials { get; set; }
        #endregion
        #region Commands
        public Command AuthorizeCommand
        {
            get
            {
                return _authorizeCommand ??
                  (_authorizeCommand = new Command(obj =>
                  {
                      Authorize();
                  }));
            }
        }
        #endregion
        #region Methods
        private async void Authorize()
        {
            try
            {
                ConnectorApp.Instance.WindowViewModel.ShowBusyScreen("Авторизация...");
                RestService restService = new RestService();
                Session newSession = new Session();
                TokenInfo? tokenInfo = await restService.GetTokenInfoAsync(Сredentials);
                if (tokenInfo is null)
                    throw new Exception("Ошибка при авторизации. Не удалось десериализовать данные авторизации.");
                newSession.Token = tokenInfo;
                restService.Token = tokenInfo.Token;

                ConnectorApp.Instance.WindowViewModel.ShowBusyScreen("Получение настроек...");
                UnitedSettings? unitedSettings =  await restService.GetUnitedSettingsAsync();
                if(unitedSettings is null)
                    throw new Exception("Ошибка при авторизации. Не удалось десериализовать данные общих настроек.");
                newSession.UnitedSettings = unitedSettings;

                ConnectorApp.Instance.WindowViewModel.ShowBusyScreen("Получение пользователя...");
                AppUser? user = await restService.GetUserFullAsync();
                if (user is null)
                    throw new Exception("Ошибка при авторизации. Не удалось десериализовать данные пользователя.");
                newSession.User = user;

                ConnectorApp.Instance.Session = newSession;
                ConnectorApp.Instance.VisualScheme = new WpfVisualScheme(ConnectorApp.Instance.Session.User.VisualScheme);                
                ServerListControlViewModel serversVm = new ServerListControlViewModel();
                await ConnectorApp.Instance.WindowViewModel.ChangeUIControl(new ServerListControl(serversVm), true);
                ConnectorApp.Instance.VisualScheme.Apply();
                ConnectorApp.Instance.WindowViewModel.HideBusyScreen();
            }
            catch (Exception ex)
            {
                AuraMessageWindow message = new AuraMessageWindow(
                    new AuraMessageWindowViewModel(
                        "Ошибка авторизации",
                        ex.Message,
                        "Ok",
                        AuraMessageWindowViewModel.MessageTypes.Error));
                    message.ShowDialog();

                ConnectorApp.Instance.WindowViewModel.HideBusyScreen();
            }
        }
        #endregion
    }
}
