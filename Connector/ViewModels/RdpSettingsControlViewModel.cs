using AuraS.Controls.ControlsViewModels;
using AuraS.Controls;
using Connector.Models.Commands;
using Connector.Models.REST;
using Connector.View;
using ConnectorCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connector.ViewModels
{
    public class RdpSettingsControlViewModel :Notifier
    {
        public RdpSettingsControlViewModel()
        {
            if (ConnectorApp.Instance.Session is null)
                throw new Exception("Ошибка при вызове редактора настроек RDP. Отсутствует сессия.");
            _backup = (RdpSettings)ConnectorApp.Instance.Session.User.UserSettings.RdpSettings.Clone();
            RdpSettings = ConnectorApp.Instance.Session.User.UserSettings.RdpSettings;
        }
        private RdpSettings _backup;
        private RdpSettings _settings;
        private Command _saveCommand;
        private Command _cancelCommand;
        private Command _resetCommand;
        public RdpSettings RdpSettings
        {
            get
            {
                return _settings;
            }
            set
            {
                _settings = value;
                OnPropertyChanged("RdpSettings");
            }
        }
        public Command SaveCommand
        {
            get
            {
                return _saveCommand ??
                    (_saveCommand = new Command(async obj =>
                    {
                        try
                        {
                            if (ConnectorApp.Instance.Session is null)
                                throw new Exception("Попытка установить настройки RDP без сессии. Переавторизуйтесь.");
                            if (ConnectorApp.Instance.Session.User is null)
                                throw new Exception("Попытка установить настройки RDP без указания пользователя. Переавторизуйтесь.");
                            if (ConnectorApp.Instance.Session.User.UserSettings is null)
                                ConnectorApp.Instance.Session.User.UserSettings = UserSettings.GetDefault();
                            ConnectorApp.Instance.Session.User.UserSettings.RdpSettings = RdpSettings;
                            await ConnectorApp.Instance.WindowViewModel.ChangeUIControl(
                                new ServerListControl(new ServerListControlViewModel()), true);
                            if (ConnectorApp.Instance.Session is not null)
                                if (ConnectorApp.Instance.Session.Token is not null)
                                {
                                    RestService restService = new RestService(ConnectorApp.Instance.Session!.Token!.Token);
                                    await restService.SendRdpSettingsAsync(ConnectorApp.Instance.Session.User.UserSettings.RdpSettings);
                                }
                                else throw new Exception("Не удалось отправить новую визуальную схему на сервер. Отсутствует токен. Необходима переавторизация.");
                            else throw new Exception("Не удалось отправить новую визуальную схему на сервер. Отсутствует сессия. Необходима переавторизация.");
                        }
                        catch (Exception ex)
                        {
                            AuraMessageWindow message = new AuraMessageWindow(
                                        new AuraMessageWindowViewModel(
                                            "Ошибка сохранения настроек RDP",
                                            ex.Message,
                                            "Ok",
                                            AuraMessageWindowViewModel.MessageTypes.Error));
                            message.ShowDialog();
                            await ConnectorApp.Instance.WindowViewModel.ChangeUIControl(
                                new ServerListControl(new ServerListControlViewModel()), true);
                        }
                    }));
            }
        }
        public Command ResetCommand
        {
            get
            {
                return _resetCommand ??
                    (_resetCommand = new Command(obj =>
                    {
                        try
                        {
                            RdpSettings = RdpSettings.GetDefault();
                        }
                        catch (Exception ex)
                        {
                            AuraMessageWindow message = new AuraMessageWindow(
                                        new AuraMessageWindowViewModel(
                                            "Ошибка сброса настроек",
                                            ex.Message,
                                            "Ok",
                                            AuraMessageWindowViewModel.MessageTypes.Error));
                            message.ShowDialog();
                        }
                    }));
            }
        }
        public Command CancelCommand
        {
            get
            {
                return _cancelCommand ??
                    (_cancelCommand = new Command(async obj =>
                    {
                        try
                        {
                            ConnectorApp.Instance.WindowViewModel.ShowBusyScreen("Отмена изменений", true);
                            RdpSettings = _backup;
                            ConnectorApp.Instance.Session.User.UserSettings.RdpSettings = RdpSettings;
                            await ConnectorApp.Instance.WindowViewModel.ChangeUIControl(
                                new ServerListControl(new ServerListControlViewModel()), true);
                        }
                        catch (Exception ex)
                        {
                            AuraMessageWindow message = new AuraMessageWindow(
                                        new AuraMessageWindowViewModel(
                                            "Ошибка восстановления настроек",
                                            ex.Message,
                                            "Ok",
                                            AuraMessageWindowViewModel.MessageTypes.Error));
                            message.ShowDialog();
                        }
                    }));
            }
        }
        public int[] ColorDepths
        {
            get
            {
                return new int[] { 8, 16, 24, 32 };
            }
        }
    }
}
