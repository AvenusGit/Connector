using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConnectorCenter.Models.Settings;

namespace ConnectorCenter.Views.AppUserGroups
{
    public class IndexModel : PageModel
    {
        public IndexModel(IEnumerable<AppUserGroup> userGroups, AccessSettings accessSettings)
        {
            UserGroups = userGroups;
            AccessSettings = accessSettings;
        }
        public IEnumerable<AppUserGroup> UserGroups { get; set; }
        public AccessSettings AccessSettings { get; set; }
    }
}
