using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ConnectorCenter.Models.Settings;
using ConnectorCore.Models.Connections;

namespace ConnectorCenter.Views.Settings
{
    public class LogsModel : PageModel
    {
        public LogsModel(LogSettings logsSettings, bool allowEdit)
        {
            LogSettings = logsSettings;
            AllowEdit = allowEdit;
        }
        public LogSettings LogSettings { get; set; }
        public bool AllowEdit { get; set; }
    }
}
