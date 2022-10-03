using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConnectorCenter.Models.Settings;
using ConnectorCore.Models.Connections;

namespace ConnectorCenter.Views.Settings
{
    public class MySettingsModel : PageModel
    {
        public MySettingsModel(AppUser user)
        {
            CurrentUser = user;
        }
        public AppUser CurrentUser { get; set; }
    }
}
