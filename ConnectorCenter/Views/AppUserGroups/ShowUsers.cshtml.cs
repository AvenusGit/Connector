using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.AppUserGroups
{
    public class ShowUsersModel : PageModel
    {
        public ShowUsersModel(AppUserGroup group)
        {
            Group = group;
        }
        public AppUserGroup Group { get; set; }
    }
}
