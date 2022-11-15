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
using ConnectorCore.Cryptography;

namespace Connector.ViewModels
{
    public class LoginControllerViewModel : Notifier
    {
        #region Constructors
        public LoginControllerViewModel() 
        {
            Credentials = new Credentials(string.Empty, string.Empty);
        }
        public LoginControllerViewModel(Credentials credentials)
        {
            Credentials = credentials;
        }
        #endregion
        #region Fields
        private Credentials _credentials;
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
        public Credentials Credentials
        { 
            get
            {
                return _credentials;
            }
            set
            {
                _credentials = value;
                if(!String.IsNullOrEmpty(_credentials.Password))
                    _credentials.Password = PasswordCryptography.GetUserPasswordHash(
                        _credentials.Login,
                        _credentials.Password);
            }
        }
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
                if (String.IsNullOrEmpty(Credentials.Login) || String.IsNullOrEmpty(Credentials.Password))
                    throw new Exception("Не все необходимые данные введены.");

                ConnectorApp.Instance.WindowViewModel.ShowBusyScreen("Авторизация...");
                RestService restService = new RestService();
                Session newSession = new Session();
                Credentials hashedCredentials = new Credentials()
                {
                    Login = Credentials.Login,
                    Password = PasswordCryptography.GetUserPasswordHash(
                        Credentials.Login,
                        Credentials.Password)
                };
                TokenInfo? tokenInfo = await restService.GetTokenInfoAsync(hashedCredentials);
                if (tokenInfo is null)
                    throw new Exception("Ошибка при авторизации. Не удалось десериализовать данные авторизации.");
                newSession.Token = tokenInfo;
                restService.Token = tokenInfo.Token;
                ConnectorApp.Instance.IsTokenOld = false;

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
                ConnectorApp.Instance.Session.User.Credentials = Credentials;
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
