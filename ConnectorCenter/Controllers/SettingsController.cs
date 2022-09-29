﻿using Microsoft.AspNetCore.Mvc;
using ConnectorCenter.Views.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using ConnectorCenter.Data;
using Microsoft.EntityFrameworkCore;
using ConnectorCenter.Models.Settings;
using ConnectorCenter.Services.Authorize;
using ConnectorCenter.Services.Configurations;
using ConnectorCore.Models;
using ConnectorCenter.Services.Logs;

namespace ConnectorCenter.Controllers
{
    /// <summary>
    /// Контроллер для работы с меню настроек
    /// </summary>
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class SettingsController : Controller
    {
        #region Fields
        private readonly ILogger _logger;
        #endregion
        #region Constructors
        public SettingsController(ILogger<AppUserGroupsController> logger)
        {
            _logger = logger;
        }
        #endregion
        #region GET
        [HttpGet]
        public IActionResult Index()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    // любой пользователь может попасть в свои настройки как минимум, проверка прав не требуется
                    _logger.LogInformation($"Запрошена страница меню настроек.");
                    return View("Index", 
                        new IndexViewModel(
                            AuthorizeService.GetAccessSettings(HttpContext),
                            AuthorizeService.GetUserRole(HttpContext) ?? AppUser.AppRoles.User
                        ));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы меню настроек. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы меню настроек.",
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
        public IActionResult Access()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    if (AuthorizeService.GetUserRole(HttpContext) == AppUser.AppRoles.Administrator)
                    {
                        _logger.LogInformation($"Запрошена страница настроек доступа.");
                        return View(new AccessModel(ConnectorCenterApp.Instance.SupportAccessSettings));
                    }                        
                    else
                    {
                        _logger.LogWarning($"Отказанов запросе страницы настроек доступа. Пользователь не является администратором.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\settings");
                    }
                        
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы настроек доступа. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы настроек доступа.",
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
        public IActionResult Logs()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.SettingsLogs == AccessSettings.AccessModes.None)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу настроек логов. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\servers");
                    }
                    _logger.LogInformation($"Запрошена страница настроек логов.");
                    return View(new LogsModel(ConnectorCenterApp.Instance.LogSettings, accessSettings.SettingsLogs == AccessSettings.AccessModes.Edit));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы настроек логов. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы настроек логов.",
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
        public IActionResult Api()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    
                    if (AuthorizeService.GetAccessSettings(HttpContext).SettingsAPI != AccessSettings.AccessModes.None)
                    {
                        _logger.LogInformation($"Запрошена страница настроек Api.");
                        return View(new ApiModel(ConnectorCenterApp.Instance.ApiSettings, AuthorizeService.GetAccessSettings(HttpContext)));
                    }
                    else
                    {
                        _logger.LogWarning($"Отказанов запросе страницы настроек Api. Нет у роли пользователя нет доступа.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\settings");
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы настроек Api. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы настроек Api.",
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
        public IActionResult IE()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {

                    if (AuthorizeService.GetAccessSettings(HttpContext).ImportAndExport)
                    {
                        _logger.LogInformation($"Запрошена страница импорта и экспорта.");
                        return View("ImportAndExport", new IEModel(AuthorizeService.GetAccessSettings(HttpContext),
                            AuthorizeService.GetUserRole(HttpContext)));
                    }
                    else
                    {
                        _logger.LogWarning($"Отказано в запросе страницы импорта и экспорта. У роли пользователя нет доступа.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\settings");
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы импорта и экспорта. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы импорта и экспорта.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"Назад",@"\settings" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }
            }
        }
        #endregion
        #region POST
        [HttpPost]
        public IActionResult SaveSupportAccessSettings(AccessSettings? supportAccessSettings)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    if (AuthorizeService.GetUserRole(HttpContext) != AppUser.AppRoles.Administrator)
                    {
                        _logger.LogWarning("Отказано в попытке изменить параметры доступа техподдержки. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\settings");
                    }
                    if (supportAccessSettings is null || !ModelState.IsValid)
                    {
                        _logger.LogError($"Ошибка при попытке сохранить настройки доступа техподдержки. Неверные аргументы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке сохранить настройки доступа техподдержки. Неверные аргументы.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    ConnectorCenterApp.Instance.SupportAccessSettings = supportAccessSettings;
                    SettingsConfigurationService.SaveConfiguration(ConnectorCenterApp.Instance.SupportAccessSettings);
                    _logger.LogInformation($"Изменены настройки доступа техподдержки.");
                    return View("Index", 
                        new IndexViewModel(
                            AuthorizeService.GetAccessSettings(HttpContext),
                            AuthorizeService.GetUserRole(HttpContext) ?? AppUser.AppRoles.User
                        ));                    
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при попытке сохранить настройки доступа техподдержки. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке сохранить настройки доступа техподдержки.",
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
        [HttpPost]
        public IActionResult SaveLogsSettings(LogSettings? logSettings)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.SettingsLogs != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке изменить параметры логгера. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\settings");
                    }
                    if (logSettings is null || !ModelState.IsValid)
                    {
                        _logger.LogError($"Ошибка при попытке сохранить настройки логгера. Неверные аргументы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке сохранить настройки логгера. Неверные аргументы.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    if (String.IsNullOrWhiteSpace(logSettings.LogPath))
                        logSettings.LogPath = LogService.GetLastLogFilePath();
                    ConnectorCenterApp.Instance.LogSettings = logSettings;
                    ConnectorCenterApp.Instance.LogSettings.SaveConfiguration();
                    _logger.LogInformation($"Изменены настройки логгера.");
                    return View("Index",
                        new IndexViewModel(
                            AuthorizeService.GetAccessSettings(HttpContext),
                            AuthorizeService.GetUserRole(HttpContext) ?? AppUser.AppRoles.User
                        ));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при попытке сохранить логгера. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке сохранить настройки логгера.",
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
        [HttpPost]
        public IActionResult SaveApiSettings(ApiSettings? apiSettings)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    if (AuthorizeService.GetAccessSettings(HttpContext).SettingsAPI != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке изменить параметры API. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\settings");
                    }
                    if (apiSettings is null || !ModelState.IsValid)
                    {
                        _logger.LogError($"Ошибка при попытке сохранить настройки API. Неверные аргументы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке сохранить настройки API. Неверные аргументы.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    ConnectorCenterApp.Instance.ApiSettings = apiSettings;
                    SettingsConfigurationService.SaveConfiguration(ConnectorCenterApp.Instance.ApiSettings);
                    _logger.LogInformation($"Изменены настройки API.");
                    return View("Index",
                        new IndexViewModel(
                            AuthorizeService.GetAccessSettings(HttpContext),
                            AuthorizeService.GetUserRole(HttpContext) ?? AppUser.AppRoles.User
                        ));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при попытке сохранить настройки API. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке сохранить настройки API.",
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
