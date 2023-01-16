using ConnectorCenter.Data;
using ConnectorCenter.Services.Authorize;
using ConnectorCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;

namespace ConnectorCenter.Controllers.Api
{
    /// <summary>
    /// Api for token authorization
    /// </summary>
    [Route("/api/token")]
    [AllowAnonymous]
    [ApiController]
    public class TokenApi : ControllerBase
    {
        #region Fields
        private readonly DataBaseContext _dataBaseContext;
        private readonly ILogger _logger;
        #endregion
        #region Constructors
        public TokenApi(DataBaseContext context, ILogger<TokenApi> logger)
        {
            _dataBaseContext = context;
            _logger = logger;
        }
        #endregion
        #region Requests
        /// <summary>
        /// API request access JWT token
        /// Token need for other requests
        /// </summary>
        /// <param name="credentials">JSON Credentials</param>
        /// <returns>JSON TokenInfo</returns>
        [Route("/api/token/gettoken")]
        [Produces(MediaTypeNames.Application.Json)]
        [Consumes("application/json")]
        [HttpPost]
        public async Task GetToken([FromBody] Credentials credentials)
        {
            using (var scope = _logger.BeginScope($"API({JwtAuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    if (!ConnectorCenterApp.Instance.ApiSettings.ApiEnabled)
                    {
                        HttpContext.Response.StatusCode = 403;
                        _logger.LogWarning($"Попытка авторизации при выключенном API. Отказано. Логин:{credentials.Login}");
                        await HttpContext.Response.WriteAsync("API disabled in app settings.");
                        return;
                    }
                    if (!ConnectorCenterApp.Instance.ApiSettings.AuthorizeApiEnabled)
                    {
                        HttpContext.Response.StatusCode = 403;
                        _logger.LogWarning($"Попытка авторизации при выключенном API авторизации. Отказано. Логин:{credentials.Login}");
                        await HttpContext.Response.WriteAsync("API JWT token authorization disabled in app settings.");
                        return;
                    }
                    if (!ModelState.IsValid)
                    {
                        _logger.LogWarning($"Попытка авторизации с некорректными аргументами. Логин:{credentials.Login}");
                        await HttpContext.Response.WriteAsync("Your model is not valid. Use {string Login, string Password}");
                        return;
                    }
                    using (DataBaseContext db = _dataBaseContext)
                    {
                        AppUser? user;
                        if (AuthorizeService.IsAuthorized(_dataBaseContext, credentials, out user))
                        {
                            if(!user.IsEnabled)
                            {
                                Response.StatusCode = 451;
                                _logger.LogWarning($"Отказ в API авторизации. Пользователь деактивирован. Логин:{credentials.Login}.");
                                await HttpContext.Response.WriteAsync("No active user");
                                return;
                            }
                            JwtSecurityToken jwt = JwtAuthorizeService.GetJwtToken(user!);
                            Response.StatusCode = 200;
                            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                            user!.Credentials.Password = null;
                            string json = JsonConvert.SerializeObject(
                                new TokenInfo(handler.WriteToken(jwt))
                                {
                                    UserName = user!.Name ?? string.Empty,
                                });
                            _logger.LogWarning($"Успешная API авторизация. Логин:{credentials.Login}.");
                            await HttpContext.Response.WriteAsync(json);
                            return;
                        }
                        else
                        {
                            Response.StatusCode = 401;
                            _logger.LogWarning($"Отказ в API авторизации. Неверные учетные данные. Логин:{credentials.Login}.");
                            await HttpContext.Response.WriteAsync("Wrong login or password");
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.StatusCode = 500;
                    _logger.LogWarning($"Ошибка при попытке API авторизации. {ex.Message} {ex.StackTrace}. Логин:{credentials.Login}.");
                    await HttpContext.Response.WriteAsync("500: Server internal error");
                }
            }
        }
        #endregion
    }
}
