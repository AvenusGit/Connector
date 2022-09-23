using Newtonsoft.Json.Linq;
using System.Diagnostics.Metrics;
using time = System.Timers;
namespace ConnectorCenter.Models.Statistics
{
    public class Statistic
    {
        public Statistic()
        {
            _timer = new time.Timer(1000);
            _timer.Elapsed += OnTimerTick;
            _timer.Start();
            _appStarted = DateTime.Now;
    }
        #region Fields
        private time.Timer _timer;
        private int _tickCounter;
        private int _currentSecond;
        private int _currentMinute;
        private int _currentHour;
        private DateTime _appStarted;
        #endregion
        #region Properties
        public long Requests { get; private set; } = 0;
        public long WebRequest { get; private set; } = 0;
        public long ApiRequest { get; private set; } = 0;
        public Queue<int> MinutesQueue { get; set; } = new Queue<int>(new int[60]);
        public Queue<int> MinutesQueueWithCurrent 
        {
            get
            {
                Queue<int> currentMinutesQueue = new Queue<int>(MinutesQueue);
                currentMinutesQueue.Enqueue(_currentSecond);
                return currentMinutesQueue;
            }
        }
        public Queue<int> HourQueue { get; set; } = new Queue<int>(new int[60]);
        public Queue<int> HourQueueWithCurrent
        {
            get
            {
                Queue<int> currentHourQueue = new Queue<int>(HourQueue);
                currentHourQueue.Enqueue(_currentMinute);
                return currentHourQueue;
            }
        }
        public Queue<int> DayQueue { get; set; } = new Queue<int>(new int[24]);
        public Queue<int> DayQueueWithCurrent
        {
            get
            {
                Queue<int> currentDayQueue = new Queue<int>(DayQueue);
                currentDayQueue.Enqueue(_currentHour);
                return currentDayQueue;
            }
        }
        public double AverageInMinute
        {
            get
            {
                try
                {
                    return MinutesQueue.Average();
                }
                catch
                {
                    return 0d;
                }
                
            }
        }
        public TimeSpan UpTime
        {
            get
            {
                return DateTime.Now - _appStarted;
            }
        }
        public string JsUpTime
        {
            get
            {
                return $"{UpTime.Days}:{UpTime.Hours}:{UpTime.Minutes}:{UpTime.Seconds}";
            }
        }
        #endregion
        #region Methods
        public void IncWebRequest()
        {
            Requests += 1;
            WebRequest += 1;
            IncRequest();
        }
        public void IncApiRequest()
        {
            Requests += 1;
            ApiRequest +=1;
            IncRequest();
        }
        private void IncRequest()
        {
            _currentSecond += 1;
            _currentMinute += 1;
            _currentHour += 1;
        }
        public void Clear()
        {
            Requests = 0;
            WebRequest = 0;
            ApiRequest = 0;
            MinutesQueue = new Queue<int>(new int[60]);
            HourQueue = new Queue<int>(new int[60]);
            DayQueue = new Queue<int>(new int[24]);
        }
        #endregion
        #region Events
        private void OnTimerTick(Object source, time.ElapsedEventArgs e)
        {
            try
            {
                _tickCounter++;
                if(_currentSecond > 0)
                {

                }
                MinutesQueue.Enqueue(_currentSecond);
                if(MinutesQueue.Count > 60)
                    MinutesQueue.Dequeue();

                _currentSecond = 0;

                if (_tickCounter >= 3600)
                {
                    HourQueue.Enqueue(_currentMinute);
                    if(HourQueue.Count > 60)
                        HourQueue.Dequeue();

                    _currentMinute = 0;
                }

                if (_tickCounter >= 86400)
                {
                    DayQueue.Enqueue(_currentHour);
                    if (DayQueue.Count > 24)
                        DayQueue.Dequeue();

                    _currentHour = 0;
                    _tickCounter = 0;
                }
            }
            catch { }
        }
        #endregion
    }
}
