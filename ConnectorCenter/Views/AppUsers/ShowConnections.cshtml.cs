using ConnectorCenter.Models.Settings;
using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.AppUsers
{
    public class ShowConnectionsModel : PageModel
    {
        public ShowConnectionsModel(AppUser user, AccessSettings accessSettings)
        {
            User = user;
            AccessSettings = accessSettings;
        }
        public AppUser User { get; set; }
        public AccessSettings AccessSettings { get; set; }
    }
}
