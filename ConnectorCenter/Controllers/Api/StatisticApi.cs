using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ConnectorCenter.Models.Statistics;
using ConnectorCenter.Data;
using ConnectorCenter.Services.Authorize;
using Newtonsoft.Json;

namespace ConnectorCenter.Controllers.Api
{
    [Route("api/statistic")]
    [Authorize(AuthenticationSchemes = "Cookies")]
    [ApiController]
    public class StatisticApi : ControllerBase
    {
        private readonly ILogger _logger;
        public StatisticApi( ILogger<AppUserGroupsController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public async Task GetStatistics()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                // статистика сама себя не считает
                try
                {
                    HttpContext.Response.StatusCode = 200;
                    HttpContext.Response.ContentType = "application/json";
                    await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(ConnectorCenterApp.Instance.Statistics));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе API статистики. {ex.Message}. {ex.StackTrace}");
                    HttpContext.Response.StatusCode = 500;
                    await HttpContext.Response.WriteAsync("Internal error 500");
                }
            }
        }
    }
}
