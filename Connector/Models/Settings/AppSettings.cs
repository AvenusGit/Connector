using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connector.Models.Settings
{
    public class AppSettings
    {
        public AppSettings()
        {
            SetDefaultSettings();
        }
        public string MainServerUrl { get; set; }
        private void SetDefaultSettings()
        {
            MainServerUrl = "127.0.0.1/api";
        }
    }
}
