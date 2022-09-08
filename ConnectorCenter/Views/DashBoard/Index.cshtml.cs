using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConnectorCore.Models;
using ConnectorCenter.Models.Settings;

namespace ConnectorCenter.Views.DashBoard
{
    public class IndexViewModel : PageModel
    {
        public IndexViewModel(AccessSettings accessSettings)
        {
            AccessSettings = accessSettings;
        }
        public AccessSettings AccessSettings { get; set; }
    }
}
