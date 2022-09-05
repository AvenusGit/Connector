using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConnectorCore.Models.Connections;

namespace ConnectorCenter.Views.AppUsers
{
    public class IndexModel : PageModel
    {
        public IndexModel(IEnumerable<AppUser> users)
        {
            Users = users;
            foreach (AppUser user in users)
            {
                foreach (AppUserGroup group in user.Groups)
                {
                    GroupConnectionsCount = GroupConnectionsCount + group.GroupConnections.Count;
                }
            }
        }
        public IEnumerable<AppUser> Users { get; set; }
        public int GroupConnectionsCount { get; set; } = 0;
    }
}
