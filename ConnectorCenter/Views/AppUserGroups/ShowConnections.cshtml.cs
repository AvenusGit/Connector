using ConnectorCenter.Models.Settings;
using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.AppUserGroups
{
    public class ShowConnectionsModel : PageModel
    {
        public ShowConnectionsModel(AppUserGroup group, AccessSettings accessSettings)
        {
            Group = group;
            AccessSettings = accessSettings;
        }
        public AppUserGroup Group { get; set; }
        public AccessSettings AccessSettings { get; set; }
    }
}
