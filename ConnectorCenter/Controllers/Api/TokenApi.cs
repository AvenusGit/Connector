using ConnectorCenter.Data;
using ConnectorCenter.Services.Authorize;
using ConnectorCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;

namespace ConnectorCenter.Controllers.Api
{
    [Route("/api/token")]
    [AllowAnonymous]
    [ApiController]
    public class TokenApi : ControllerBase
    {
        private readonly DataBaseContext _dataBaseContext;
        private readonly ILogger _logger;
        public TokenApi(DataBaseContext context, ILogger logger)
        {
            _dataBaseContext = context;
            _logger = logger;
    }

        [HttpGet]
        private async Task Index()
        {
            if (!ConnectorCenterApp.Instance.ApiSettings.ApiEnabled)
            {
                HttpContext.Response.StatusCode = 403;
                await HttpContext.Response.WriteAsync("API disabled in app settings.");
                return;
            }
            if (!ConnectorCenterApp.Instance.ApiSettings.AuthorizeApiEnabled)
            {
                HttpContext.Response.StatusCode = 403;
                await HttpContext.Response.WriteAsync("API JWT token authorization disabled in app settings.");
                return;
            }
            Response.StatusCode = 400;
            await HttpContext.Response.WriteAsync("JSON (Credentials{string Login, string Password}) in request body need!");
        }

        [Route("/api/token/gettoken")]
        [Produces(MediaTypeNames.Application.Json)]
        [Consumes("application/json")]
        [HttpPost]
        public async Task GetToken([FromBody] Сredentials credentials)
        {
            using (var scope = _logger.BeginScope($"API({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
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
                            JwtSecurityToken jwt = JwtAuthorizeService.GetJwtToken(user!);
                            Response.StatusCode = 200;
                            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                            var json = new
                            {
                                token = handler.WriteToken(jwt),
                                user = user!.Name
                            };
                            _logger.LogWarning($"Успешная API авторизация. Логин:{credentials.Login}.");
                            await HttpContext.Response.WriteAsJsonAsync(json);
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
    }
}
