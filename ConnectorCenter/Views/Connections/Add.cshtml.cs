using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConnectorCenter.Models;
using ConnectorCore.Models.Connections;

namespace ConnectorCenter.Views.Connections
{
    public class AddModel : PageModel
    {
        public AddModel(Server server)
        {
            Server = server;
        }
        public Server Server { get; set; }
    }
}
