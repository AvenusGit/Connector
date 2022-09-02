using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.AppUserGroups
{
    public class IndexModel : PageModel
    {
        public IndexModel(IEnumerable<AppUserGroup> userGroups)
        {
            UserGroups = userGroups;
        }
        public IEnumerable<AppUserGroup> UserGroups { get; set; }
    }
}
