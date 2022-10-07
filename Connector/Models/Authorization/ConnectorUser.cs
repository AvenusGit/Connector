using Connector.Models.REST;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using Aura.VisualModels;
using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using ConnectorCore.Models.VisualModels;

namespace Connector.Models.Authorization
{
    public class ConnectorUser :  AppUser
    {
       
        public ConnectorUser() { }
        public ConnectorUser(string name, Сredentials credentials)
        {
            Name = name;
            Credentials = credentials;
        }
        public WpfVisualScheme VisualScheme { get; set; }
        
        //public async Task UpdateConnections(Action before, Action after)
        //{
        //    before?.Invoke();
        //    RestService restService = new RestService();
        //    IEnumerable<Connection> connectionList = await restService.GetConnectionListAsync(ConnectorApp.Instance.CurrentUser.Id);
        //    after?.Invoke();
        //    //Connections = new ObservableCollection<Connection>(connectionList); //TODO
        //}
        //public async Task UpdateUserSettings(Action before, Action after)
        //{
        //    before?.Invoke();
        //    RestService restService = new RestService();
        //    UserSettings = await restService.GetUserSettingsAsync(Id);
        //    after?.Invoke();
        //}
        //public async Task UpdateVisualSettings(Action before, Action after)
        //{
        //    before?.Invoke();
        //    RestService restService = new RestService();
        //    WpfVisualScheme visualSettings = await restService.GetVisualSchemeAsync(Id);
        //    after?.Invoke();
        //    VisualScheme = new WpfVisualScheme()
        //    {
        //        ColorScheme = visualSettings.ColorScheme,
        //        FontScheme = visualSettings.FontScheme
        //    };

        //}
    }
}