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

namespace Connector.Models.Authorization
{
    public class ConnectorUser : IUser
    {
        public ConnectorUser() { }
        public ConnectorUser(string name, Сredentials credentials, IUser.Roles role,
            ConnectorSettings settings = null!)
        {
            Name = name;
            Credentials = credentials;
            Role = role;
            UserSettings = settings;
        }

        public string Name { get; set; }
        public Сredentials Credentials { get; set; }
        public IEnumerable<RdpInfo> Connections { get; set; }
        public ConnectorSettings UserSettings { get; set; }
        public IVisualScheme VisualScheme { get; set; }
        public IUser.Roles Role { get; set; }
        public async Task UpdateConnections(Action before, Action after)
        {
            before?.Invoke();
            ConnectorRestService restService = new ConnectorRestService();
            IEnumerable<RdpInfo> connectionList = await restService.GetServerListAsync(ConnectorApp.Instance.CurrentUser);
            after?.Invoke();
            Connections = new ObservableCollection<RdpInfo>(connectionList);
        }
        public async Task UpdateUserSettings(Action before, Action after)
        {
            before?.Invoke();
            ConnectorRestService restService = new ConnectorRestService();
            ISettings userSettings = await restService.GetUserSettingsAsync(this);
            after?.Invoke();
            UserSettings = new ConnectorSettings()
            {
                MainServer = userSettings.MainServer,
            };
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