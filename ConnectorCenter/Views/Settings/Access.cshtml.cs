using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConnectorCenter.Models.Settings;
using ConnectorCore.Models.Connections;

namespace ConnectorCenter.Views.Settings
{
    public class AccessModel : PageModel
    {
        public AccessModel(AccessSettings supportAccessSettings)
        {
            SupportAccessSettings = supportAccessSettings;
        }
        public AccessSettings SupportAccessSettings { get; set; }
    }
}
