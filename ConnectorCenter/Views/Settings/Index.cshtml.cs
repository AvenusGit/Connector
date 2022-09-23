using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConnectorCore.Models;
using ConnectorCenter.Models.Settings;

namespace ConnectorCenter.Views.Settings
{
    public class IndexViewModel : PageModel
    {
        public IndexViewModel(AccessSettings accessSettings, AppUser.AppRoles userRole)
        {
            AccessSettings = accessSettings;
            UserRole = userRole;
        }
        public AccessSettings AccessSettings { get; set; }
        public AppUser.AppRoles UserRole { get; set; }
    }
}
