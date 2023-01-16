using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ConnectorCenter.Models.Statistics;
using ConnectorCenter.Data;
using ConnectorCenter.Services.Authorize;
using Newtonsoft.Json;

namespace ConnectorCenter.Controllers.Api
{
    /// <summary>
    /// Api for statistic requests
    /// </summary>
    [Authorize(AuthenticationSchemes = "Cookies")]
    [ApiController]
    public class StatisticApi : ControllerBase
    {
        #region Fields and Properties
        private readonly ILogger _logger;
        #endregion
        #region Constructors
        public StatisticApi(ILogger<AppUserGroupsController> logger)
        {
            _logger = logger;
        }
        #endregion
        #region Requests

        /// <summary>
        /// API request full statistics
        /// </summary>
        /// <returns>JSON statistic</returns>
        [HttpGet]
        [Route("api/statistic/all")]
        public async Task GetStatistics()
        {
            using (var scope = _logger.BeginScope($"API({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                // request statistics not increment statistic counter
                try
                {
                    if (!ConnectorCenterApp.Instance.ApiSettings.ApiEnabled)
                    {
                        _logger.LogWarning($"Отказано в запросе API статистики. API глобально выключен.");
                        HttpContext.Response.StatusCode = 403;
                        await HttpContext.Response.WriteAsync("API disabled in app settings.");                   
                        return;
                    }
                    if (!ConnectorCenterApp.Instance.ApiSettings.StatisticApiEnabled)
                    {
                        _logger.LogWarning($"Отказано в запросе API статистики. API статистики выключено.");
                        HttpContext.Response.StatusCode = 403;
                        await HttpContext.Response.WriteAsync("API statistics disabled in app settings.");
                        return;
                    }

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

        /// <summary>
        /// API request minute statistic
        /// </summary>
        /// <returns>JSON statistic part</returns>
        [HttpGet]
        [Route("api/statistic/min")]
        public async Task GetStatisticsMin()
        {
            using (var scope = _logger.BeginScope($"API({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                // request statistics not increment statistic counter
                try
                {
                    if (!ConnectorCenterApp.Instance.ApiSettings.ApiEnabled)
                    {
                        _logger.LogWarning($"Отказано в запросе API статистики. API глобально выключен.");
                        HttpContext.Response.StatusCode = 403;
                        await HttpContext.Response.WriteAsync("API disabled in app settings.");
                        return;
                    }
                    if (!ConnectorCenterApp.Instance.ApiSettings.StatisticApiEnabled)
                    {
                        _logger.LogWarning($"Отказано в запросе API статистики. API статистики выключено.");
                        HttpContext.Response.StatusCode = 403;
                        await HttpContext.Response.WriteAsync("API statistics disabled in app settings.");
                        return;
                    }
                    HttpContext.Response.StatusCode = 200;
                    HttpContext.Response.ContentType = "application/json";
                    await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(
                            ConnectorCenterApp.Instance.Statistics.GetPart(StatisticPart.StatisticMode.Min)
                        ));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе API статистики за минуту. {ex.Message}. {ex.StackTrace}");
                    HttpContext.Response.StatusCode = 500;
                    await HttpContext.Response.WriteAsync("Internal error 500");
                }
            }
        }

        /// <summary>
        /// API request hour statistic
        /// </summary>
        /// <returns>JSON statistic part</returns>
        [HttpGet]
        [Route("api/statistic/hour")]
        public async Task GetStatisticsHour()
        {
            using (var scope = _logger.BeginScope($"API({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                // request statistics not increment statistic counter
                try
                {
                    if (!ConnectorCenterApp.Instance.ApiSettings.ApiEnabled)
                    {
                        _logger.LogWarning($"Отказано в запросе API статистики. API глобально выключен.");
                        HttpContext.Response.StatusCode = 403;
                        await HttpContext.Response.WriteAsync("API disabled in app settings.");
                        return;
                    }
                    if (!ConnectorCenterApp.Instance.ApiSettings.StatisticApiEnabled)
                    {
                        _logger.LogWarning($"Отказано в запросе API статистики. API статистики выключено.");
                        HttpContext.Response.StatusCode = 403;
                        await HttpContext.Response.WriteAsync("API statistics disabled in app settings.");
                        return;
                    }
                    HttpContext.Response.StatusCode = 200;
                    HttpContext.Response.ContentType = "application/json";
                    await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(
                            ConnectorCenterApp.Instance.Statistics.GetPart(StatisticPart.StatisticMode.Hour)
                        ));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе API статистики за час. {ex.Message}. {ex.StackTrace}");
                    HttpContext.Response.StatusCode = 500;
                    await HttpContext.Response.WriteAsync("Internal error 500");
                }
            }
        }

        /// <summary>
        /// API request day statistic
        /// </summary>
        /// <returns>JSON statistic part</returns>
        [HttpGet]
        [Route("api/statistic/day")]
        public async Task GetStatisticsDay()
        {
            using (var scope = _logger.BeginScope($"API({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                // request statistics not increment statistic counter
                try
                {
                    if (!ConnectorCenterApp.Instance.ApiSettings.ApiEnabled)
                    {
                        _logger.LogWarning($"Отказано в запросе API статистики. API глобально выключен.");
                        HttpContext.Response.StatusCode = 403;
                        await HttpContext.Response.WriteAsync("API disabled in app settings.");
                        return;
                    }
                    if (!ConnectorCenterApp.Instance.ApiSettings.StatisticApiEnabled)
                    {
                        _logger.LogWarning($"Отказано в запросе API статистики. API статистики выключено.");
                        HttpContext.Response.StatusCode = 403;
                        await HttpContext.Response.WriteAsync("API statistics disabled in app settings.");
                        return;
                    }
                    HttpContext.Response.StatusCode = 200;
                    HttpContext.Response.ContentType = "application/json";
                    await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(
                            ConnectorCenterApp.Instance.Statistics.GetPart(StatisticPart.StatisticMode.Day)
                        ));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе API статистики за день. {ex.Message}. {ex.StackTrace}");
                    HttpContext.Response.StatusCode = 500;
                    await HttpContext.Response.WriteAsync("Internal error 500");
                }
            }
        }
        #endregion
    }
}
