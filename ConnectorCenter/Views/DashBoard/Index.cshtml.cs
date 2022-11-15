using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConnectorCore.Models;
using ConnectorCenter.Models.Settings;

namespace ConnectorCenter.Views.DashBoard
{
    public class IndexViewModel : PageModel
    {
        public IndexViewModel(AccessSettings accessSettings, string username, AppUser.AppRoles userRole)
        {
            AccessSettings = accessSettings;
            UserName = username;
            UserRole = userRole;
        }
        public AccessSettings AccessSettings { get; set; }
        public string UserName { get; set; }
        public AppUser.AppRoles UserRole { get; set; }
    }
}
