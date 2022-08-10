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
using ConnectorCore.Models.Connections;
using ConnectorCore.Models.Server;
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
                Name = "authorizeUser",
                Credentials = сredentials,
                Role = IApplicationUser.AppRoles.Administrator
            };
        }
        public async Task<IUser?> AuthorizeAsync(Сredentials сredentials)
        {
            await Task.Delay(1000);
            return Authorize(сredentials);
        }
        public IEnumerable<IConnection> GetServerList(IUser user)
        {
            //TODO test method
            return new List<IConnection>()
                {
                    new RdpConnection()
                    {
                        ConnectionName = "RdpTestConnection",
                        Locked = false,
                        Server = new ServerInfo()
                        {
                            Name = "TestRdpServer",
                            HostOrIP = "127.0.0.1",
                            Port = 3389
                        },
                        User = new RdpUser()
                        {
                            Name = "rdpTestUser1",
                            Credentials = new Сredentials("login", "pass"),
                            Role = RdpUser.RdpRoles.User
                        }
                    },
                    new SshConnection()
                    {
                        ConnectionName = "SshTestConnection",
                        Locked = false,
                        Сredentials = new Сredentials("login", "password"),
                        Server = new ServerInfo()
                        {
                            Name = "TestServer 2",
                            HostOrIP = "127.0.0.2",
                            Port = 3389,
                        }
                    }
                };
        }
        public async Task<IEnumerable<IConnection>> GetConnectionListAsync(IUser user)
        {
            //TODO
            await Task.Delay(1200);
            return GetServerList(user);
        }
        public IUserSettings? GetSettings(IUser user)
        {
            return new ConnectorUserSettings()
            {
                ConnectorServer = new ServerInfo()
                {
                    HostOrIP = "127.0.0.1",
                    Name = "MainServer",
                    Port = 80
                },
            };
        }
        public async Task<IUserSettings?> GetUserSettingsAsync(IUser user)
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
