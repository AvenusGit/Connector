using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConnectorCore.Models.Connections;
using ConnectorCenter.Models.Settings;

namespace ConnectorCenter.Views.AppUsers
{
    public class IndexModel : PageModel
    {
        public IndexModel(IEnumerable<AppUser> users, AccessSettings accessSettings)
        {
            Users = users;
            AccessSettings = accessSettings;
        }
        public IEnumerable<AppUser> Users { get; set; }
        public AccessSettings AccessSettings { get; set; }
    }
}
