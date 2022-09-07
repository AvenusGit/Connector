using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.AppUserGroups
{
    public class ShowConnectionsModel : PageModel
    {
        public ShowConnectionsModel(AppUserGroup group)
        {
            Group = group;
        }
        public AppUserGroup Group { get; set; }
    }
}