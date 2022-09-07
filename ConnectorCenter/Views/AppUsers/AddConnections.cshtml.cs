using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.AppUsers
{
    public class AddConnectionsModel : PageModel
    {
        public AddConnectionsModel(IEnumerable<Server> fullConnectionList, AppUser user)
        {
            User = user;
            FullServerList = fullConnectionList;

            GroupConnections = new List<Connection>();
            foreach (AppUserGroup group in user.Groups)
            {
                GroupConnections.AddRange(group.Connections);
            }
            GroupConnections = GroupConnections.Distinct().ToList();
        }
        public new AppUser User { get; set; }
        public IEnumerable<Server> FullServerList { get; set; }
        public List<Connection> GroupConnections { get; set; }
    }
}
