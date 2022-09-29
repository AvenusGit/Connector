using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConnectorCenter.Models.Settings;
using ConnectorCore.Models.Connections;

namespace ConnectorCenter.Views.Settings
{
    public class ApiModel : PageModel
    {
        public ApiModel(ApiSettings apiSettings, AccessSettings accessSettings)
        {
            ApiSettings = apiSettings;
            AccessSettings = accessSettings;
        }
        public ApiSettings ApiSettings { get; set; }
        public AccessSettings AccessSettings { get; set; }
    }
}
