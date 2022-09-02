using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.Servers
{
    public class IndexModel : PageModel
    {
        public IndexModel(IEnumerable<Server> servers)
        {
            Servers = servers;
        }
        public IEnumerable<Server> Servers { get; set; }
    }
}
