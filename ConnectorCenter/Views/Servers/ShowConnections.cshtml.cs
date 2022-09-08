using ConnectorCenter.Models.Settings;
using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.Servers
{
    public class ShowConnectionsModel : PageModel
    {
        public ShowConnectionsModel(Server server, AccessSettings accessSettings)
        {
            Server = server;
            AccessSettings = accessSettings;
        }
        public Server Server { get; set; }
        public AccessSettings AccessSettings { get; set; }
    }
}
