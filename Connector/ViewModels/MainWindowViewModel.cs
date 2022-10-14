using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Connector.Models.Commands;
using Connector.View;
using System.Diagnostics;

namespace Connector.ViewModels
{
    public class MainWindowViewModel : Notifier
    {
        public MainWindowViewModel(UserControl startcontrol, Window mainWindow)
        {
            CurrentUserControl = startcontrol;
            MainWindow = mainWindow;
            ConnectorApp.Instance.WindowViewModel = this;
        }
        #region Fields
        private bool _isBusy;
        private string _busyMessage;
        private bool _showBusyIndicator;

        private Command _closeCommand;
        private Command _minimizeCommand;
        private Command _settingsCommand;
        private Command _toGit;
        private UserControl userControl;
        #endregion
        #region Commands
        public Command ExitCommand
        {
            get
            {
                return _closeCommand ??
                  (_closeCommand = new Command(obj =>
                  {
                      ApplicationExit();
                  }));
            }
        }
        public Command MinimizeCommand
        {
            get
            {
                return _minimizeCommand ??
                  (_minimizeCommand = new Command(obj =>
                  {
                      MinimizeApplication();
                  }));
            }
        }
        public Command ToSettingsCommand
        {
            get
            {
                return _settingsCommand ??
                  (_settingsCommand = new Command(obj =>
                  {
                      if(CurrentUserControl is not AppSettingsControl)
                        GoToSettings();
                  }));
            }
        }
        public Command ToGitCommand
        {
            get
            {
                return _toGit ??
                  (_toGit = new Command(obj =>
                  {
                      Process.Start(new ProcessStartInfo
                      {
                          FileName = "https://github.com/AvenusGit/Connector",
                          UseShellExecute = true
                      });
                  }));
            }
        }
        #endregion
        #region Methods
        public void ShowBusyScreen(string message, bool showBusyIndicator = true)
        {
            BusyMessage = message;
            IsBusy = true;
            ShowBusyIndicator = showBusyIndicator;
        }
        public void HideBusyScreen()
        {
            IsBusy = false;
            BusyMessage = string.Empty;
        }

        public async Task ChangeUIControl(UserControl newControl, bool showAnimation)
        {
            if (showAnimation)
            {
                ShowBusyScreen("", false);
                await Task.Delay(200);
                ConnectorApp.Instance.WindowViewModel.CurrentUserControl = newControl;
                HideBusyScreen();
            }
            else
                ConnectorApp.Instance.WindowViewModel.CurrentUserControl = newControl;

            OnPropertyChanged("ShowConnectionSettingsButton");
        }

        private void ApplicationExit()
        {
            Environment.Exit(0);
        }
        private void MinimizeApplication()
        {
            MainWindow.WindowState = WindowState.Minimized;
        }
        private async void GoToSettings()
        {
            await ChangeUIControl(new AppSettingsControl(CurrentUserControl), true);
        }
        #endregion
        #region Properties
        public Window MainWindow
        {
            get; private set;
        }
        public UserControl CurrentUserControl
        {
            get { return userControl; }
            set
            {
                userControl = value;
                OnPropertyChanged("CurrentUserControl");
            }
        }

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            private set
            {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }
        public string BusyMessage 
        {
            get
            {
                return _busyMessage;
            }
            private set
            {
                _busyMessage = value;
                OnPropertyChanged("BusyMessage");
            }
        }
        public bool ShowBusyIndicator
        {
            get
            {
                return _showBusyIndicator;
            }
            set
            {
                _showBusyIndicator = value;
                OnPropertyChanged("ShowBusyIndicator");
            }
        }

        public string Title
        {
            get
            {
                return ConnectorApp.AppName;
            }
        }
        public string FooterText
        {
            get
            {
                return ConnectorApp.AppVersion + ", " + DateTime.Now.Year;
            }
        }
        public string HeaderText
        {
            get
            {
                return Title + " " + ConnectorApp.AppVersion;
            }
        }
        public bool ShowConnectionSettingsButton
        {
            get
            {
                return CurrentUserControl is LoginControl;
            }
        }
        #endregion
    }
}
