using ConnectorCore.Interfaces;
using ConnectorCore.Models.Authorization;
using Connector.Models.Settings;
using ConnectorCore.Models.Server;
using Connector.Models.REST;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using AuraS.Interfaces;
using AuraS.Models;
using ConnectorCore.Interfaces;

namespace Connector.Models.Authorization
{
    public class ConnectorUser : IApplicationUser
    {
        public ConnectorUser() { }
        public ConnectorUser(string name, Сredentials credentials)
        {
            Name = name;
            Credentials = credentials;
        }

        public string Name { get; set; }
        public Сredentials Credentials { get; set; }
        public IEnumerable<IConnection> Connections { get; set; }
        public IUserSettings UserSettings { get; set; }
        public IVisualScheme VisualScheme { get; set; }
        public IApplicationUser.AppRoles Role { get; set; }
        public async Task UpdateConnections(Action before, Action after)
        {
            before?.Invoke();
            ConnectorRestService restService = new ConnectorRestService();
            IEnumerable<IConnection> connectionList = await restService.GetConnectionListAsync(ConnectorApp.Instance.CurrentUser);
            after?.Invoke();
            Connections = new ObservableCollection<IConnection>(connectionList);
        }
        public async Task UpdateUserSettings(Action before, Action after)
        {
            before?.Invoke();
            ConnectorRestService restService = new ConnectorRestService();
            UserSettings = await restService.GetUserSettingsAsync(this);
            after?.Invoke();
        }
        public async Task UpdateVisualSettings(Action before, Action after)
        {
            before?.Invoke();
            ConnectorRestService restService = new ConnectorRestService();
            IVisualScheme visualSettings = await restService.GetVisualSchemeAsync(this);
            after?.Invoke();
            VisualScheme = new VisualScheme()
            {
                ColorScheme = visualSettings.ColorScheme,
                FontScheme = visualSettings.FontScheme
            };

        }
    }
}