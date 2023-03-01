using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConnectorCenter.Views.Logs
{
    public class IndexModel : PageModel
    {
        public IndexModel(string logs)
        {
            Logs = logs;
        }
        public string Logs { get; set; }
    }
}
