namespace ConnectorCore.Models
{
    public class ServerGroup
    {
        public long Id { get; set; }
        public string? GroupName { get; set; }
        public IEnumerable<Server> Servers { get; set; } = new List<Server>();
    }
}
