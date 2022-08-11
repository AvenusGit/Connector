using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using ConnectorCore.Models;
using AuraS.Interfaces;
using AuraS.Styles.DefaultStyles;

namespace Connector.Models.REST
{
    public class RestService
    {
        public HttpClient HttpClient { get; set; }
        //private string JwtToken;
        public AppUser? Authorize(Сredentials сredentials)
        {
            return new AppUser()
            {
                Name = "authorizeUser",
                Credentials = сredentials,
                Role = AppUser.AppRoles.Administrator
            };
        }
        public async Task<AppUser?> AuthorizeAsync(Сredentials сredentials)
        {
            await Task.Delay(1000);
            return Authorize(сredentials);
        }
        public IEnumerable<Connection> GetServerList(long userId)
        {
            //TODO test method
            return new List<Connection>()
                {
                    new Connection()
                    {
                        ConnectionName = "RdpTestConnection",
                        Locked = false,
                        Server = new ServerInfo()
                        {
                            Host = "127.0.0.1",
                            Port = 3389
                        },
                        User = new ServerUser()
                        {
                            Name = "rdpTestUser1",
                            Credentials = new Сredentials("login", "pass"),
                            Role = ServerUser.Roles.User
                        }
                    },
                    new Connection()
                    {
                        ConnectionName = "SshTestConnection",
                        Locked = false,
                        Server = new ServerInfo()
                        {
                            Host = "127.0.0.1",
                            Port = 3389
                        },
                        User = new ServerUser()
                        {
                            Name = "sshTestUser1",
                            Credentials = new Сredentials("login", "pass"),
                            Role = ServerUser.Roles.Administrator
                        }
                    }
                };
        }
        public async Task<IEnumerable<Connection>> GetConnectionListAsync(long userId)
        {
            //TODO
            await Task.Delay(1200);
            return GetServerList(userId);
        }
        public UserSettings? GetSettings(long userId)
        {
            return new UserSettings()
            {
                // TODO Settings
            };
        }
        public async Task<UserSettings?> GetUserSettingsAsync(long userId)
        {
            await Task.Delay(900);
            return GetSettings(userId);
        }
        public IVisualScheme? GetVisualScheme(long userId)
        {
            return DefaultStyleGenerator.GetDefaultVisualScheme();
        }
        public async Task<IVisualScheme?> GetVisualSchemeAsync(long userId)
        {
            await Task.Delay(1500);
            return GetVisualScheme(userId);
        }
    }
}
