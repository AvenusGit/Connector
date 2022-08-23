using ConnectorCenter.Services.Authorize;
using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.Login
{
    public class IndexModel : PageModel
    {
        public IndexModel(HttpContext context, string? message)
        {
            UserName = context.User.Identity?.Name;
            Message = message;
        }
        public string? Message { get; set; }
        public string? UserName { get; set; }
    }
}
