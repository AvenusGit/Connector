using ConnectorCenter.Models.Settings;
using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.AppUsers
{
    public class EditModel : PageModel
    {
        public EditModel(AppUser user, AccessSettings accessSettings)
        {
            User = user;
            AccessSettings = accessSettings;
        }
        public AppUser User { get; set; }
        public AccessSettings AccessSettings { get; set; }
    }
}
