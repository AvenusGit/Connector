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
using ConnectorCenter.Models.Repository;

namespace ConnectorCenter.Controllers
{
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class StatisticController : Controller
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly AppUserGroupRepository _appUserGroupRepository;
        private readonly AppUserRepository _userRepository;
        private readonly ServerRepository _serverRepository;
        private readonly ConnectionRepository _connectionRepository;
        #endregion
        #region Constructors
        public StatisticController(ILogger<AppUserGroupsController> logger,
            AppUserGroupRepository appUserGroupRepository,
            AppUserRepository userRepository,
            ServerRepository serverRepository,
            ConnectionRepository connectionRepository)
        {
            _logger = logger;
            _appUserGroupRepository = appUserGroupRepository;
            _userRepository = userRepository;
            _serverRepository = serverRepository;
            _connectionRepository = connectionRepository;
        }
        #endregion
        #region GET
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    AccessSettings currentAcessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if(currentAcessSettings is not null)
                    {
                        if (!currentAcessSettings.Statistics)
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
                            await _appUserGroupRepository.Count(),
                            await _userRepository.Count(),
                            await _serverRepository.Count(),
                            await _connectionRepository.Count())) ;
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
                    AccessSettings currentAcessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (currentAcessSettings is not null)
                    {
                        if (!currentAcessSettings.Statistics)
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
                            await _appUserGroupRepository.Count(),
                            await _userRepository.Count(),
                            await _serverRepository.Count(),
                            await _connectionRepository.Count()));
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
        #endregion
    }
}
