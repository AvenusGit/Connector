using ConnectorCenter.Models.Settings;
using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.Servers
{
    public class IndexModel : PageModel
    {
        public IndexModel(IEnumerable<Server> servers, AccessSettings accessSettings)
        {
            Servers = servers;
            AccessSettings = accessSettings;
        }
        public IEnumerable<Server> Servers { get; set; }
        public AccessSettings AccessSettings { get; set; }
    }
}
