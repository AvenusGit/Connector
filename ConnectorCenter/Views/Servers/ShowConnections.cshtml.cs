using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.Servers
{
    public class ShowConnectionsModel : PageModel
    {
        public ShowConnectionsModel(Server server)
        {
            Server = server;
        }
        public Server Server { get; set; }
    }
}
