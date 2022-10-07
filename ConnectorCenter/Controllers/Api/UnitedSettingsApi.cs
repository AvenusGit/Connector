using ConnectorCenter.Data;
using ConnectorCenter.Services.Authorize;
using ConnectorCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mime;

namespace ConnectorCenter.Controllers.Api
{
    [Route("/api/unitedSettings")]
    [Authorize(AuthenticationSchemes = "Bearer")]   
    [ApiController]
    public class UnitedSettingsApi : ControllerBase
    {
        private readonly ILogger _logger;
        public UnitedSettingsApi(ILogger<TokenApi> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public async Task GetUnitedSettings()
        {
            using (var scope = _logger.BeginScope($"API({JwtAuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    if (!ConnectorCenterApp.Instance.ApiSettings.ApiEnabled)
                    {
                        HttpContext.Response.StatusCode = 403;
                        await HttpContext.Response.WriteAsync("API disabled in app settings.");
                        return;
                    }
                    Response.StatusCode = 200;
                    await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(ConnectorCenterApp.Instance.OtherSettings));
                }
                catch (Exception ex)
                {
                    Response.StatusCode = 500;
                    _logger.LogWarning($"Ошибка при попытке API общих настроек. {ex.Message} {ex.StackTrace}.");
                    await HttpContext.Response.WriteAsync("500: Server internal error");
                }
            }
        }        
    }
}
