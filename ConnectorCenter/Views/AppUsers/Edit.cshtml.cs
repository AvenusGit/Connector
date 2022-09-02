using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.AppUsers
{
    public class EditModel : PageModel
    {
        public EditModel(AppUser user)
        {
            User = user;
        }
        public AppUser User { get; set; }
    }
}
