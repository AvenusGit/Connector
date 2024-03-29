using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.AppUserGroups
{
    public class AddConnectionsModel : PageModel
    {
        public AddConnectionsModel(IEnumerable<Server> fullConnectionList, AppUserGroup group)
        {
            Group = group;
            FullServerList = fullConnectionList;
        }
        public new AppUserGroup Group { get; set; }
        public IEnumerable<Server> FullServerList { get; set; }
    }
}
