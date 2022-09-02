using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.AppUserGroups
{
    public class EditUserGroupsModel : PageModel
    {
        public EditUserGroupsModel(AppUserGroup userGroup)
        {
            Group = userGroup;
        }
        public AppUserGroup Group { get; set; }
    }
}
