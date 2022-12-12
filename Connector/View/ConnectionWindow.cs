using ConnectorCore.Models.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;

namespace Connector.View
{
    public abstract class ConnectionWindow : Window, INotifyPropertyChanged
    {
        public Connection Connection { get; set; }
        public abstract void Connect(object? sender, EventArgs e);
        private bool _isBusy;
        private string _busyMessage;
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
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
            set
            {
                _busyMessage = value;
                OnPropertyChanged("BusyMessage");
            }
        }
        public string WindowLogo { get; set; }
        private void DragWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch { }
        }
        public void OnMinimize(object? sender, EventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        #region InotifyPropertyChange
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}
