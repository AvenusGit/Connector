using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConnectorCenter.Data;
using ConnectorCenter.Views.Statistics;
using ConnectorCore.Models.Connections;
using ConnectorCore.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using ConnectorCenter.Services.Authorize;
using ConnectorCenter.Models.Settings;

namespace ConnectorCenter.Controllers
{
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class StatisticController : Controller
    {
        #region Fields
        private readonly AccessSettings _accessSettings;
        private readonly ILogger _logger;
        private readonly DataBaseContext _context;
        #endregion
        #region Constructors
        public StatisticController(DataBaseContext context,ILogger<AppUserGroupsController> logger)
        {
            _logger = logger;
            _context = context;
            _accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
        }
        #endregion
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if(_accessSettings is not null)
                    {
                        if (!_accessSettings.Statistics)
                        {
                            _logger.LogWarning("Отказано в попытке запросить страницу статистики. Недостаточно прав.");
                            return AuthorizeService.ForbiddenActionResult(this, @"\dashboard");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Ошибка при запросе страницы статистики. Не удалось определить роль пользователя.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе страницы статистики. Не удалось определить роль пользователя.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    _logger.LogInformation($"Запрошена страница статистики.");
                    return View(
                        new IndexModel(
                            await _context.UserGroups.CountAsync(),
                            await _context.Users.CountAsync(),
                            await _context.Servers.CountAsync(),
                            await _context.Connections.CountAsync()));
                }
                catch(Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы статистики. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы статистики.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }
                
            }
        }
        [HttpGet]
        public async Task<IActionResult> Clear()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    if (_accessSettings is not null)
                    {
                        if (!_accessSettings.Statistics)
                        {
                            _logger.LogWarning("Отказано в попытке очистить статистику приложения. Недостаточно прав.");
                            return AuthorizeService.ForbiddenActionResult(this, @"\dashboard");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Отказано в попытке очистить статистику приложения. Неопределенная роль пользователя.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Отказано в попытке очистить статистику приложения. Неопределенная роль пользователя.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }                                      
                    ConnectorCenterApp.Instance.Statistics.Clear();
                    _logger.LogInformation($"Очищена статистика приложения.");
                    return View( "Index",
                        new IndexModel(
                            await _context.UserGroups.CountAsync(),
                            await _context.Users.CountAsync(),
                            await _context.Servers.CountAsync(),
                            await _context.Connections.CountAsync()));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при попытке очистить статичтику приложения. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке очистить статичтику приложения.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }
            }
        }
    }
}
