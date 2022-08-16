using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.Login
{
    public class IndexModel : PageModel
    {
        public IndexModel(string? message)
        {
            Message = message;
        }
        public string? Message { get; set; }
    }
}
