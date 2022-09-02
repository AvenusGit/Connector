using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.AppUsers
{
    public class IndexModel : PageModel
    {
        public IndexModel(IEnumerable<AppUser> users)
        {
            Users = users;
        }
        public IEnumerable<AppUser> Users { get; set; }
    }
}
