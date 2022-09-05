using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.AppUserGroups
{
    public class AddUserModel : PageModel
    {
        public AddUserModel(IEnumerable<AppUser> fullUsersList, AppUserGroup group)
        {
            Group = group;
            FullUsersList = fullUsersList;
        }
        public new AppUserGroup Group { get; set; }
        public IEnumerable<AppUser> FullUsersList { get; set; }
    }
}
