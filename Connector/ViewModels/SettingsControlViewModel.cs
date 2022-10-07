using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connector.Models.Commands;
using Aura.VisualModels;
using Connector.View;
using ConnectorCore.Models.VisualModels.Interfaces;

namespace Connector.ViewModels
{
    public class SettingsControlViewModel
    {
        public SettingsControlViewModel()
        {
            //if(ConnectorApp.Instance.CurrentUser.VisualScheme is WpfVisualScheme)
            //    _backup = (ConnectorApp.Instance.CurrentUser.VisualScheme as WpfVisualScheme).Clone();
        }
        private Command _saveCommand;
        private Command _cancelCommand;
        private WpfVisualScheme _backup;
        public Command SaveCommand
        {
            get
            {
                return _saveCommand ??
                    (_saveCommand = new Command(async obj =>
                    {
                        _backup.Apply();
                        await ConnectorApp.Instance.WindowViewModel.ChangeUIControl(
                            new ServerListControl(new ServerListControlViewModel()), true);
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
                        await ConnectorApp.Instance.WindowViewModel.ChangeUIControl(
                            new ServerListControl(new ServerListControlViewModel()), true);
                    }));
            }
        }
    }
}
