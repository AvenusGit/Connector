namespace ConnectorCenter.Models.Statistics
{
    public struct StatisticPart
    {
        public StatisticMode Mode { get; set; }
        public long Requests { get; set; }
        public long WebRequest { get; set; }
        public long ApiRequest { get; set; }
        public double AverageInMinute { get; set; }
        public Queue<int> StatisticQueue { get; set; }
        public string UpTime { get; set; }
        public enum StatisticMode
        {
            Min,
            Hour,
            Day
        }
    }
}
