using ConnectorCore.Models.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Extensions;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ConnectorCore.Models;
using Renci.SshNet;
using Connector.Models;
using Renci.SshNet.Common;
using System.IO;
using System.Threading;
using Connector.Models.Commands;

namespace Connector.View
{
    public partial class SshWindow : ConnectionWindow
    {
        public SshWindow(Connection connection, IEnumerable<Connection> otherSshVariants)
        {
            this.DataContext = this;
            Connection = connection;
            WindowLogo = $"SSH:{Connection.Server.Name}:{Connection.ConnectionName}";
            InitializeComponent();
            LastCommands = new List<string>();
            History = new Queue<string>();
            OtherVariants = otherSshVariants;
            OnPropertyChanged("OtherVariants");
            Loaded += Connect;            
        }
        private int lastCommandIndex = 0;
        private Queue<string> _history;
        public IEnumerable<Connection> OtherVariants { get; private set; }
        public string HistoryString 
        { 
            get
            {
                StringBuilder builder = new StringBuilder();
                if(History.Count == 0)
                    return String.Empty;
                foreach (string historySshCommand in History)
                {
                    if (builder.ToString() == String.Empty)
                        builder.Append(historySshCommand);
                    else
                        builder.Append(Environment.NewLine + historySshCommand);
                }
                iConsoleScrollViewer.ScrollToEnd();                
                return builder.ToString();
            }
        }
        public string CurrentCommand { get; set; }
        public List<string> LastCommands { get; set; }
        public Queue<string> History 
        {
            get
            {
                if(_history is null)
                    _history = new Queue<string>();
                return _history;
            }
            set
            {
                _history = value;
                OnPropertyChanged("History");
                OnPropertyChanged("HistoryString");
            }
        }
        public SshClient Client { get; set; }
        public ShellStream SshStream { get; set; }
        public bool IsBlocked { get; set; }
        public override void Connect(object? sender, EventArgs e)
        {
            BusyMessage = "Идет подключение...";
            IsBusy = true;

            if (Connection is null)
                throw new Exception("Не удалось подключиться к SSH серверу. Подключение не определено. Обратитесь к администратору.");
            if (Connection.ServerUser.Credentials is null)
                throw new Exception("Не удалось подключиться к SSH серверу. Не указаны данные авторизации. Обратитесь к администратору.");
            if (String.IsNullOrEmpty(Connection.ServerUser.Credentials.Login))
                throw new Exception("Не удалось подключиться к SSH серверу. Не указан логин пользователя. Обратитесь к администратору.");
            if (String.IsNullOrEmpty(Connection.Server.Host))
                throw new Exception("Не удалось подключиться к SSH серверу. Не указан хост. Обратитесь к администратору.");            

            if(ConnectorApp.Instance.Session is null || ConnectorApp.Instance.Session.User is null)
            {
                Close(this, null);
                AuraS.Controls.AuraMessageWindow message = new AuraS.Controls.AuraMessageWindow(new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel
                    ("Ошибка при подключении", "Отсутствует сессия пользователя. Переавторизауйтесь в приложении.", "Ок",
                    AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Error));
                ConnectorApp.Instance.Logout();
                return;
            }
            if (ConnectorApp.Instance.Session.User.UserSettings is null)
            {
                Close(this, null);
                AuraS.Controls.AuraMessageWindow message = new AuraS.Controls.AuraMessageWindow(new AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel
                    ("Ошибка при подключении", "Отсутствуют настройки пользователя. Они будут назначены по умолчанию.", "Ок",
                    AuraS.Controls.ControlsViewModels.AuraMessageWindowViewModel.MessageTypes.Warning));
                ConnectorApp.Instance.Session.User.UserSettings = UserSettings.GetDefault();
            }

            try
            {
                PasswordConnectionInfo connectionInfo = new PasswordConnectionInfo(
                    Connection.Server.Host,
                    Connection.ServerUser.Credentials.Login,
                    Encoding.UTF8.GetBytes(Connection.ServerUser.Credentials.Password)
                    );

                Client = new SshClient(connectionInfo);
                Client.Connect();
                if(Client.IsConnected)
                {
                    IsBlocked = true;
                    AddToHistory($"Подключено к {Connection.Server.Name} под пользователем {Connection.ServerUser.Name}.", false);
                    SshStream = Client.CreateShellStream("terminal", 200, 40, 500, 200, 1024);
                    SshStream.DataReceived += SshDataRecieved;
                    IsBusy = false;
                    IsBlocked = false;
                }
                else
                {
                    AddToHistory($"Не удалось подключиться к {Connection.Server.Name} под пользователем {Connection.ServerUser.Name}.", false);
                    IsBusy = false;
                    IsBlocked = true;
                }                          
            }
            catch (Exception ex)
            {
                AddToHistory($"Ошибка: {ex.Message}.", false);
                Client.Dispose();
                IsBusy = false;
                IsBlocked = true;
            }            
        }
        private void Maximized(object sender, RoutedEventArgs e)
        {
            if(this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }
        private void Minimize(object sender, RoutedEventArgs e)
        {
            OnMinimize(sender, e);
        }
        private void Close(object sender, RoutedEventArgs e)
        {
            Client.Dispose();
            this.Close();
            ConnectorApp.Instance.ActiveConnections.Remove(this);
        }

        private void ConsoleKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Up)
            {
                if (LastCommands.ElementAtOrDefault(LastCommands.Count -1 - lastCommandIndex) is not null)
                {
                    CurrentCommand = LastCommands.ElementAtOrDefault(LastCommands.Count - 1 - lastCommandIndex) ?? "";
                    lastCommandIndex += 1;
                    OnPropertyChanged("CurrentCommand");
                }                   
                return;
            }
            if (e.Key is Key.Down)
            {
                if (lastCommandIndex >= 0 && LastCommands.Count > 0)
                {
                    CurrentCommand = LastCommands.ElementAtOrDefault(LastCommands.Count - 1 - lastCommandIndex) 
                        ?? LastCommands.ElementAtOrDefault(LastCommands.Count -lastCommandIndex - 1) ?? "";
                    lastCommandIndex = lastCommandIndex > 0
                        ? lastCommandIndex - 1 
                        : lastCommandIndex = 0;
                    OnPropertyChanged("CurrentCommand");
                }                   
                return;
            }
            lastCommandIndex = 0;
            if (e.Key is Key.Enter)
            {
                AddToLastCommand(CurrentCommand);
                switch (CurrentCommand)
                {
                    case "cls":
                        ClearHistory();
                        AddToHistory("История команд очищена.");
                        CurrentCommand = "";
                        return;
                    case "clear":
                        ClearHistory();
                        AddToHistory("История команд очищена.");
                        CurrentCommand = "";
                        return;
                    default:
                        break;
                }
                if (Client.IsConnected)
                {                    
                    SshStream.WriteLine(CurrentCommand);                              
                }
                else
                {
                    AddToHistory("Нет подключения. Переподключитесь к серверу.");
                    IsBusy = false;
                    IsBlocked = true;
                }
                CurrentCommand = "";
                OnPropertyChanged("CurrentCommand");                
            }            
        }
        private void AddToHistory(string text, bool isUserCommand = false)
        {
            if (History.Count >= 500)
                History.Dequeue();
            if (isUserCommand)
                History.Enqueue($"[{DateTime.Now}]{Connection.ConnectionUserShort}: {text}");
            else
                History.Enqueue(text);
            OnPropertyChanged("HistoryString");
        }
        private void AddToLastCommand(string command)
        {
            if (LastCommands.Count >= 50)
                LastCommands.RemoveAt(0);
            LastCommands.Add(CurrentCommand);
        }
        private void SshDataRecieved(object? sender, ShellDataEventArgs e)
        {
            if (e.Data != null)
            {
                string recieved = Encoding.UTF8.GetString(e.Data);
                foreach (string line in recieved.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
                {
                    AddToHistory(line);
                }
            }                
        }
        private void ClearHistory()
        {
            History.Clear();
            OnPropertyChanged("HistoryString");
        }
        private void ClearLastCommands()
        {
            LastCommands.Clear();
        }
        private void Reconnect(Connection connection)
        {
            ClearHistory();
            AddToHistory($"Переподключение под пользователем {connection.ServerUser.Name} на сервер {connection.Server.Name}...");
            this.Connection = connection;
            Connect(this, null!);
        }

        #region Commands
        private Command _maximizeCommand;
        private Command _minimizeCommand;
        private Command _closeCommand;
        private Command _reconnectCommand;
        private Command _clearCommand;
        public Command MinimizeCommand
        {
            get
            {
                return _minimizeCommand ??
                  (_minimizeCommand = new Command(obj =>
                  {
                      Minimize(this, null!);
                  }));
            }
        }
        public Command MaximizeCommand
        {
            get
            {
                return _maximizeCommand ??
                  (_maximizeCommand = new Command(obj =>
                  {
                      Maximized(this, null!);
                  }));
            }
        }
        public Command CloseCommand
        {
            get
            {
                return _closeCommand ??
                  (_closeCommand = new Command(obj =>
                  {
                      Close(this, null!);
                  }));
            }
        }
        public Command ClearCommand
        {
            get
            {
                return _clearCommand ??
                  (_clearCommand = new Command(obj =>
                  {
                      ClearHistory();
                      ClearLastCommands();
                      AddToHistory("История и список последних команд очищены");
                  }));
            }
        }
        public Command ReconnectCommand
        {
            get
            {
                return _reconnectCommand ??
                    (_reconnectCommand = new Command(obj =>
                    {
                        try
                        {
                            if (obj is Connection)
                            {
                                if ((Connection)obj == this.Connection)
                                {
                                    iConsole.Focus();
                                    AddToHistory("Это подключение сейчас активно...");
                                    return;
                                }
                                else
                                {
                                    Reconnect((Connection)obj);
                                    iConsole.Focus();
                                }                               
                            }
                            else
                                throw new Exception("Неизвестная ошибка. Параметр не является типом Connection.");
                        }
                        catch (Exception ex)
                        { }
                    }));
            }
        }
        #endregion
    }
}
