using ConnectorCenter.Models.Statistics;
using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.Statistics
{
    public class IndexModel : PageModel
    {
        public IndexModel(int groupCount, int userCount, int serverCount, int connectionsCount)
        {
            Statistic = ConnectorCenterApp.Instance.Statistics;
            GroupCount = groupCount;
            UserCount = userCount;
            ServerCount = serverCount;
            ConnectionsCount = connectionsCount;
        }
        public Statistic Statistic { get; private set; }
        public int GroupCount { get; private set; }
        public int UserCount { get; private set; }
        public int ServerCount { get; private set; }
        public int ConnectionsCount { get; private set; }
    }
}
