using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.AppUsers
{
    public class ShowConnectionsModel : PageModel
    {
        public ShowConnectionsModel(AppUser user)
        {
            User = user;
        }
        public AppUser User { get; set; }
    }
}
