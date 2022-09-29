using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConnectorCenter.Models.Settings;
using ConnectorCore.Models.Connections;

namespace ConnectorCenter.Views.Settings
{
    public class IEModel : PageModel
    {
        public IEModel(AccessSettings accessSettings, AppUser.AppRoles? role)
        {
            AccessSettings = accessSettings;
            UserRole = role;
        }
        public AccessSettings AccessSettings { get; set; }
        public AppUser.AppRoles? UserRole { get; set; }
    }
}
