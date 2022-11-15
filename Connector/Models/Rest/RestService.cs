using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using ConnectorCore.Models.VisualModels.Interfaces;
using Aura.VisualModels;
using Newtonsoft.Json;
using System.Security.Policy;
using System.Windows.Markup;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using ConnectorCore.Models.VisualModels;

namespace Connector.Models.REST
{
    public class RestService
    {
        public RestService() 
        {
            Token = null;
        }
        public RestService(string token) 
        {
            Token = token;
        }
        public string? Token { get; set; }
        public HttpClient HttpClient { get; set; } = new HttpClient();
        private async Task<HttpResponseMessage> RequestAsync(string request, HttpMethod method, bool isTokenNeed, HttpContent? content = null)
        {
            if(isTokenNeed)
                if (ConnectorApp.Instance.IsTokenOld)
                    ConnectorApp.Instance.UpdateToken();
            HttpClient = new HttpClient();
            HttpClient.Timeout = new TimeSpan(0, 0, 5);
            HttpRequestMessage message = new HttpRequestMessage(method, ConnectorApp.Instance.ConnectorCenterUrl + request);
            if (content is not null)
                message.Content = content;
                
            if(!String.IsNullOrWhiteSpace(Token))
                message.Headers.Add("Authorization", "Bearer " + Token);
            return await HttpClient.SendAsync(message);
        }
        public async Task<TokenInfo?> GetTokenInfoAsync(Credentials сredentials)
        {
            HttpResponseMessage tokenResponse = await RequestAsync(
                @"/api/token/gettoken",
                HttpMethod.Post,
                false,
                JsonContent.Create(сredentials));
            if(tokenResponse.IsSuccessStatusCode)
            {
                string resultJson = await tokenResponse.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TokenInfo>(resultJson);
            }
            else
            {
                if (tokenResponse.StatusCode == HttpStatusCode.UnavailableForLegalReasons)
                    throw new Exception("Ошибка при авторизации. Пользователь заблокирован администратором.");
                if (tokenResponse.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Exception("Ошибка при авторизации. Неверный логин пароль.");
                if (tokenResponse.StatusCode == HttpStatusCode.NotFound)
                    throw new Exception("Ошибка при авторизации. Сервер не найден.");
                else
                    throw new Exception($"Ошибка при авторизации. Проверьте адрес сервера и учетные данные.");
            }
        }
        public async Task<UnitedSettings?> GetUnitedSettingsAsync()
        {
            if(String.IsNullOrWhiteSpace(Token))
                throw new Exception("Попытка получить общие настройки не имея токена авторизации. Необходима переавторизация.");
            HttpResponseMessage response = await RequestAsync(@"/api/unitedSettings", HttpMethod.Get, true);
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<UnitedSettings>(await response.Content.ReadAsStringAsync());
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Exception("Ошибка при получении общих настроек. Пользователь не авторизован.");
                else
                {
                    throw new Exception($"{(int)response.StatusCode}:{response.Content.ReadAsStringAsync()}");
                }
            }
        }
        public async Task<IEnumerable<Connection>?> GetConnectionListAsync()
        {
            if (String.IsNullOrWhiteSpace(Token))
                throw new Exception("Попытка получить список подключений не имея токена авторизации. Необходима переавторизация.");
                    HttpResponseMessage tokenResponse = await RequestAsync(
                    @"/api/appuser/connections",
                    HttpMethod.Get,
                    true
                );
            if (tokenResponse.IsSuccessStatusCode)
            {
                string resultJson = await tokenResponse.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Connection>>(resultJson)
                            ?? null;
            }
            else
            {
                if (tokenResponse.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Exception("Ошибка при авторизации. Неверный логин пароль");
                if (tokenResponse.StatusCode == HttpStatusCode.NotFound)
                    throw new Exception("Ошибка при авторизации. Сервер не найден.");
                else
                {
                    throw new Exception($"{(int)tokenResponse.StatusCode}:{tokenResponse.Content.ReadAsStringAsync()}");
                }
            }
        }
        public async Task<IEnumerable<AppUserGroup>?> GetGroupsListAsync()
        {
            if (String.IsNullOrWhiteSpace(Token))
                throw new Exception("Попытка получить список подключений не имея токена авторизации. Необходима переавторизация.");
                HttpResponseMessage tokenResponse = await RequestAsync(
                        @"/api/appuser/groups",
                        HttpMethod.Get,
                        true
            );
            if (tokenResponse.IsSuccessStatusCode)
            {
                string resultJson = await tokenResponse.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<AppUserGroup>>(resultJson)
                            ?? null;
            }
            else
            {
                if (tokenResponse.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Exception("Ошибка при авторизации. Неверный логин пароль");
                if (tokenResponse.StatusCode == HttpStatusCode.NotFound)
                    throw new Exception("Ошибка при авторизации. Сервер не найден.");
                else
                    throw new Exception($"{(int)tokenResponse.StatusCode}:{tokenResponse.Content.ReadAsStringAsync()}");
            }
        }
        public async Task<AppUser?> GetUserFullAsync()
        {
            if (String.IsNullOrWhiteSpace(Token))
                throw new Exception("Попытка получить данные пользователя не имея токена авторизации. Необходима переавторизация.");
            HttpResponseMessage tokenResponse = await RequestAsync(
                @"/api/appuser/full",
                HttpMethod.Get,
                true
                );
            if (tokenResponse.IsSuccessStatusCode)
            {
                string resultJson = await tokenResponse.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AppUser>(resultJson);
            }
            else
            {
                if (tokenResponse.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Exception("Ошибка при авторизации. Неверный логин пароль");
                if (tokenResponse.StatusCode == HttpStatusCode.NotFound)
                    throw new Exception("Ошибка при авторизации. Сервер не найден.");
                else
                {
                    throw new Exception($"{(int)tokenResponse.StatusCode}:{tokenResponse.Content.ReadAsStringAsync()}");
                }
            }
        }
        public async Task SendVisualSchemeAsync(VisualScheme newScheme)
        {
            HttpResponseMessage tokenResponse = await RequestAsync(
                @"/api/appuser/visualScheme/set",
                HttpMethod.Post,
                true,
                JsonContent.Create(newScheme));
            if (!tokenResponse.IsSuccessStatusCode)
            {
                if (tokenResponse.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Exception("Ошибка авторизации при сохранении визуальных настроек.");
                if (tokenResponse.StatusCode == HttpStatusCode.NotFound)
                    throw new Exception("Ошибка при сохранении визуальных настроек. Сервер не найден.");
                else
                {
                    throw new Exception($"{(int)tokenResponse.StatusCode}:{tokenResponse.Content.ReadAsStringAsync()}");
                }
            }
        }
    }
}
