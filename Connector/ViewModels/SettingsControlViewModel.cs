using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connector.Models.Commands;
using Aura.VisualModels;
using AuraS.Controls;
using Connector.View;
using ConnectorCore.Models.VisualModels.Interfaces;
using Connector.Models.REST;
using AuraS.Controls.ControlsViewModels;

namespace Connector.ViewModels
{
    public class SettingsControlViewModel : Notifier
    {
        public SettingsControlViewModel(WpfVisualScheme scheme, AuraSettingControl auraControl)
        {
            CurrentScheme = scheme;
            AuraControl = auraControl;
            _backup = scheme.Clone();               
        }
        private Command _saveCommand;
        private Command _cancelCommand;
        private Command _resetCommand;
        private WpfVisualScheme _backup;
        private WpfVisualScheme _currentScheme;
        public AuraSettingControl AuraControl { get; set; }
        public WpfVisualScheme CurrentScheme
        {
            get
            {
                return _currentScheme;
            }
            set
            {
                _currentScheme = value;
                OnPropertyChanged("CurrentScheme");
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
                            CurrentScheme.Apply();
                            await ConnectorApp.Instance.WindowViewModel.ChangeUIControl(
                                new ServerListControl(new ServerListControlViewModel()), true);
                            if (ConnectorApp.Instance.Session is not null)
                                if (ConnectorApp.Instance.Session.Token is not null)
                                {
                                    RestService restService = new RestService(ConnectorApp.Instance.Session!.Token!.Token);
                                    await restService.SendVisualSchemeAsync(ConnectorApp.Instance.VisualScheme.ToVisualScheme());
                                }
                                else throw new Exception("Не удалось отправить новую визуальную схему на сервер. Отсутствует токен. Необходима переавторизация.");
                            else throw new Exception("Не удалось отправить новую визуальную схему на сервер. Отсутствует сессия. Необходима переавторизация.");
                        }
                        catch (Exception ex)
                        {
                            AuraMessageWindow message = new AuraMessageWindow(
                                        new AuraMessageWindowViewModel(
                                            "Ошибка сохранения настроек",
                                            ex.Message,
                                            "Ok",
                                            AuraMessageWindowViewModel.MessageTypes.Error));
                            message.ShowDialog();
                        }                        
                    }));
            }
        }
        public Command ResetCommand
        {
            get
            {
                return _resetCommand ??
                    (_resetCommand = new Command(async obj =>
                    {
                        try
                        {
                            CurrentScheme = ConnectorApp.Instance.VisualScheme.GetDefault();
                            ConnectorApp.Instance.VisualScheme = CurrentScheme;
                            CurrentScheme.Apply();
                            AuraControl.DataContext = new AuraSettingControlViewModel(CurrentScheme);
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
                            CurrentScheme = _backup;
                            CurrentScheme.Apply();
                            await ConnectorApp.Instance.WindowViewModel.ChangeUIControl(
                                new ServerListControl(new ServerListControlViewModel()), true);
                            ConnectorApp.Instance.WindowViewModel.HideBusyScreen();
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
    }
}
