using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connector.Models.Commands;
using Aura.VisualModels;
using Connector.View;
using ConnectorCore.Models.VisualModels.Interfaces;
using Connector.Models.REST;

namespace Connector.ViewModels
{
    public class SettingsControlViewModel
    {
        public SettingsControlViewModel()
        {
            if(ConnectorApp.Instance.VisualScheme is WpfVisualScheme)
                _backup = ConnectorApp.Instance.VisualScheme.Clone();               
        }
        private Command _saveCommand;
        private Command _cancelCommand;
        private Command _resetCommand;
        private WpfVisualScheme _backup;
        public Command SaveCommand
        {
            get
            {
                return _saveCommand ??
                    (_saveCommand = new Command(async obj =>
                    {
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
                        ConnectorApp.Instance.VisualScheme = ConnectorApp.Instance.VisualScheme.GetDefault();
                        ConnectorApp.Instance.VisualScheme.GetDefault().Apply();
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
                        ConnectorApp.Instance.WindowViewModel.ShowBusyScreen("Отмена изменений",true);
                        ConnectorApp.Instance.VisualScheme = _backup;
                        ConnectorApp.Instance.VisualScheme.Apply();
                        await ConnectorApp.Instance.WindowViewModel.ChangeUIControl(
                            new ServerListControl(new ServerListControlViewModel()), true);
                        ConnectorApp.Instance.WindowViewModel.HideBusyScreen();
                    }));
            }
        }
    }
}
