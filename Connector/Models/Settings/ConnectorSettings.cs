using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectorCore.Interfaces;
using ConnectorCore.Models.Server;
using Connector.Models.REST;
using AuraS.Models;
using AuraS.Interfaces;

namespace Connector.Models.Settings
{
    public class ConnectorUserSettings : IUserSettings
    {
        public ServerInfo ConnectorServer { get; set; }

        public async Task<IUserSettings?> GetSettings (IUser user)
        {
            ConnectorRestService restService = new ConnectorRestService();
            return await restService.GetUserSettingsAsync(user);
        }

        public async Task<IVisualScheme?> GetVisualScheme(IUser user)
        {
            ConnectorRestService restService = new ConnectorRestService();
            return await restService.GetVisualSchemeAsync(user);
        }
    }
}
