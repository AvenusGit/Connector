using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using ConnectorCore.Interfaces;
using ConnectorCore.Models.Server;
using conn=Connector.Models.Authorization;
using ConnectorCore.Models.Authorization;
using Connector.Models.Settings;
using AuraS.Models;
using AuraS.Interfaces;
using AuraS.Styles.DefaultStyles;

namespace Connector.Models.REST
{
    public class ConnectorRestService
    {
        public HttpClient HttpClient { get; set; }
        //private string JwtToken;
        public IUser? Authorize(Сredentials сredentials)
        {
            return new conn.ConnectorUser()
            {
                Name = "testConnectorUser",
                Credentials = сredentials,
                Role = IUser.Roles.Administrator
            };
        }
        public async Task<IUser?> AuthorizeAsync(Сredentials сredentials)
        {
            await Task.Delay(1000);
            return Authorize(сredentials);
        }
        public IEnumerable<RdpInfo> GetServerList(IUser user)
        {
            //TODO test method
            return new List<RdpInfo>()
                {
                    new RdpInfo()
                    {
                        ConnectionName = "TestConnection1",
                        UserName = user.Name,
                        Сredentials = new Сredentials(user.Credentials.Login,user.Credentials.Password),
                        ServerInfo = new ServerInfo()
                        {
                            Name = "TestServer 1",
                            HostOrIP = "127.0.0.1",
                            Port = 3389,
                        }
                    },
                    new RdpInfo()
                    {
                        ConnectionName = "TestConnection2",
                        UserName = "TestUser2",
                        Сredentials = new Сredentials(user.Credentials.Login,user.Credentials.Password),
                        ServerInfo = new ServerInfo()
                        {
                            Name = "TestServer 2",
                            HostOrIP = "127.0.0.2",
                            Port = 3389,
                        }
                    }
                };
        }
        public async Task<IEnumerable<RdpInfo>> GetServerListAsync(IUser user)
        {
            //TODO
            await Task.Delay(1200);
            return GetServerList(user);
        }
        public ISettings? GetSettings(IUser user)
        {
            return new ConnectorSettings()
            {
                MainServer = new ServerInfo()
                {
                    HostOrIP = "127.0.0.1",
                    Name = "MainServer",
                    Port = 80
                },
            };
        }
        public async Task<ISettings?> GetUserSettingsAsync(IUser user)
        {
            await Task.Delay(900);
            return GetSettings(user);
        }
        public IVisualScheme? GetVisualScheme(IUser user)
        {
            return DefaultStyleGenerator.GetDefaultVisualScheme();
        }
        public async Task<IVisualScheme?> GetVisualSchemeAsync(IUser user)
        {
            await Task.Delay(1500);
            return GetVisualScheme(user);
        }
    }
}
