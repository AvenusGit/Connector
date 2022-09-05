using ConnectorCenter.Services.Authorize;
using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.Message
{
    public class IndexModel : PageModel
    {
        public IndexModel(string message, Dictionary<string,string> buttons)
        {
            ButtonsDictionary = buttons;
            Message = message;
        }
        public string Message { get; set; }
        public Dictionary<string, string> ButtonsDictionary { get; set; }
    }
}
