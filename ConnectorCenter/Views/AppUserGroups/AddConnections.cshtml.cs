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
        }
        public new AppUser User { get; set; }
        public IEnumerable<Server> FullServerList { get; set; }
    }
}
