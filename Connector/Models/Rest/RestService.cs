using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using ConnectorCore.Models;
using AuraS.VisualModels;
using ConnectorCore.Models.Connections;
using ConnectorCore.Models.VisualModels.Interfaces;
using ConnectorCore.Models.VisualModels;

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
                Role = IAppUser.AppRoles.Administrator
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
                        IsAvailable = false,
                        Server = new Server()
                        {
                            Host = "127.0.0.1",                        
                        },
                        User = new ServerUser()
                        {
                            Name = "rdpTestUser1",
                            Credentials = new Сredentials("login", "pass"),
                        }
                    },
                    new Connection()
                    {
                        ConnectionName = "SshTestConnection",
                        IsAvailable = false,
                        Server = new Server()
                        {
                            Host = "127.0.0.1",                           
                        },
                        User = new ServerUser()
                        {
                            Name = "sshTestUser1",
                            Credentials = new Сredentials("login", "pass"),
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
        public VisualScheme GetVisualScheme(long userId)
        {
            return WpfVisualScheme.GetDefaultVisualScheme(); //TODO
        }
        public async Task<VisualScheme>? GetVisualSchemeAsync(long userId)
        {
            await Task.Delay(1500);
            return GetVisualScheme(userId);
        }
    }
}
