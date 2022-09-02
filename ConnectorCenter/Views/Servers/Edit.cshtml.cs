using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.Servers
{
    public class EditModel : PageModel
    {
        public EditModel(Server server)
        {
            Server = server;
        }
        public Server Server { get; set; }
    }
}
