using ConnectorCenter.Services.Authorize;
using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.Message
{
    public class IndexModel : PageModel
    {
        public IndexModel(string message, Dictionary<string,string> buttons, int? errorCode = null!)
        {
            ErrorCode = errorCode;
            ButtonsDictionary = buttons;
            Message = message;
        }
        public int? ErrorCode { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string> ButtonsDictionary { get; set; }
    }
}
