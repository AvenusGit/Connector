using System;
using Connector.Models.Commands;
using Connector.Models.REST;
using Connector.Models.Authorization;
using Connector.View;
using ConnectorCore.Models;

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
            ConnectorApp.Instance.WindowViewModel.ShowBusyScreen("Авторизация...");
            RestService restService = new RestService();
            AppUser? user =  await restService.AuthorizeAsync(Сredentials);
            if (user is not null)
            {
                ConnectorApp.Instance.CurrentUser = new ConnectorUser(
                    user.Name,
                    user.Credentials);
                await ConnectorApp.Instance.CurrentUser.UpdateConnections(
                    new Action(() =>
                    {
                        ConnectorApp.Instance.WindowViewModel.ShowBusyScreen("Получение доступов...");
                    }),
                    new Action(() =>
                    {
                        ServerListControlViewModel serversVm = new ServerListControlViewModel();
                        ConnectorApp.Instance.WindowViewModel.CurrentUserControl = new ServerListControl(serversVm);
                    }));
                await ConnectorApp.Instance.CurrentUser.UpdateUserSettings(
                    new Action(() =>
                    {
                        ConnectorApp.Instance.WindowViewModel.ShowBusyScreen("Получение настроек...");
                    }), null!
                    );
                await ConnectorApp.Instance.CurrentUser.UpdateVisualSettings(
                    new Action(() =>
                    {
                        ConnectorApp.Instance.WindowViewModel.ShowBusyScreen("Получение темы...");
                    }), 
                    new Action(() =>
                    {
                        ConnectorApp.Instance.WindowViewModel.HideBusyScreen();
                    }));
            }
            else
            {
                //TODO Message
                //AuraMessageWindow message = new AuraMessageWindow("Авторизация","Неверный логин/пароль",
                //    "Ok", AuraMessageWindow.MessageTypes.WarningType);
                //message.ShowDialog();
                ConnectorApp.Instance.WindowViewModel.HideBusyScreen();
            }

        }
        #endregion
    }
}
