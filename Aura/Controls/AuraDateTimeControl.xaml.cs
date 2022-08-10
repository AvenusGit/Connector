using System;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Aura.Controls
{
    /// <summary>
    /// Аналогичный, но улучшенный контрол DateTimeUserControl.xaml
    /// </summary>    
    public partial class AuraDateTimeControl : INotifyPropertyChanged
    {
        private DateTime _currentDateTime;
        /// <summary>
        /// Текущее время на контроле.
        /// Биндить на System.DateTime.Now напрямую нельзя
        /// </summary>
        public DateTime CurrentDateTime 
        {
            get
            {
                return _currentDateTime;
            }
            private set
            {
                _currentDateTime = value;
                OnPropertyChanged("CurrentDateTime");
            }
        }

        /// <summary>
        /// Таймер обновления
        /// </summary>
        readonly DispatcherTimer _timer;
        public AuraDateTimeControl()
        {
            InitializeComponent();
            this.DataContext = this;
            _timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            Loaded += ControlLoaded;
        }

        void ControlLoaded(object sender, RoutedEventArgs e)
        {
            //Выключаем обновление если это конструктор
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;
            _timer.Tick += TimerTick;
            _timer.Start();
            //Первично обновляем без таймера
            CurrentDateTime = DateTime.Now;
        }

        /// <summary>
        /// Событие на каждый тик таймера
        /// </summary>
        void TimerTick(object sender, EventArgs e)
        {
            CurrentDateTime = DateTime.Now;
        }

        // Далее методы для обновления привязки

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


    }
}
