using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConnectorCenter.Models.Settings;
using ConnectorCore.Models.Connections;

namespace ConnectorCenter.Views.Settings
{
    public class OtherSettingsModel : PageModel
    {
        public OtherSettingsModel(OtherSettings apiSettings, AccessSettings accessSettings)
        {
            OtherSettings = apiSettings;
            AccessSettings = accessSettings;
        }
        public OtherSettings OtherSettings { get; set; }
        public AccessSettings AccessSettings { get; set; }
    }
}
