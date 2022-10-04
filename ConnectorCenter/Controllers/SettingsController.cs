using Microsoft.AspNetCore.Mvc;
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
using ConnectorCore.Models.VisualModels;
using System.Text;

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
        private readonly DataBaseContext _context;
        #endregion
        #region Constructors
        public SettingsController(ILogger<AppUserGroupsController> logger, DataBaseContext context)
        {
            _logger = logger;
            _context = context;
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
                            AuthorizeService.GetUserRole(HttpContext)
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
        [HttpGet]
        public IActionResult Other()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    AccessSettings? accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if(accessSettings is null)
                    {
                        _logger.LogWarning($"Отказанов запросе страницы прочих настроек. Роль пользователя не определена.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\settings");
                    }
                    if (accessSettings?.SettingsOther != AccessSettings.AccessModes.None)
                    {
                        _logger.LogInformation($"Запрошена страница прочих настроек.");
                        return View(new OtherSettingsModel(ConnectorCenterApp.Instance.OtherSettings, accessSettings!));
                    }
                    else
                    {
                        _logger.LogWarning($"Отказанов запросе страницы прочих настроек. Нет у роли пользователя нет доступа.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\settings");
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы прочих настроек. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы прочих настроек.",
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
        public async Task<IActionResult> My()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    _logger.LogInformation($"Запрошена страница личных настроек.");

                    AppUser? currentUser = await _context.Users
                       .Include(user => user.UserSettings)
                       .Include(user => user.VisualScheme)
                           .ThenInclude(vs => vs.ColorScheme)
                        .Include(user => user.VisualScheme)
                            .ThenInclude(vs => vs.FontScheme)
                       .FirstOrDefaultAsync(user => user.Id == AuthorizeService.GetUserId(HttpContext));

                    if (currentUser is null)
                    {
                        _logger.LogError($"Ошибка при запросе страницы личных настроек. Пользователь не найден в БД.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы личных настроек. Пользователь не найден.",
                            buttons = new Dictionary<string, string>()
                            {
                                    {"На главную",@"\settings" },
                                    {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                    }
                    if(currentUser.UserSettings is null)
                    {
                        _logger.LogError($"Ошибка при запросе страницы личных настроек. Настройки пользователя не найдены. Будут применены стандартные настройки.");
                        currentUser.UserSettings = UserSettings.GetDefault();
                        _context.Update(currentUser);
                        _logger.LogError($"Стандартные личные настройки успешно установлены пользователю {currentUser.Name}.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе страницы личных настроек. Настройки пользователя не найдены и установлены по умолчанию.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\settings" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 500
                            }));
                    }
                    if (currentUser.VisualScheme is null || currentUser.VisualScheme.ColorScheme is null || currentUser.VisualScheme.FontScheme is null)
                    {
                        _logger.LogError($"Ошибка при запросе страницы визуальных настроек. Настройки пользователя не найдены. Будут применены стандартные настройки.");
                        currentUser.VisualScheme = VisualScheme.GetDefaultVisualScheme();
                        _context.Update(currentUser);
                        _logger.LogError($"Стандартные визуальные настройки успешно установлены пользователю {currentUser.Name}.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе страницы визуальных настроек. Настройки пользователя не найдены и установлены по умолчанию.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\settings" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 500
                            }));
                    }
                    return View(new MySettingsModel(currentUser));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы личных настроек. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы личных настроек.",
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
                            AuthorizeService.GetUserRole(HttpContext)
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
                            AuthorizeService.GetUserRole(HttpContext)
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
                            AuthorizeService.GetUserRole(HttpContext)
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
        [HttpPost]
        public IActionResult SaveOtherSettings(OtherSettings? otherSettings)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    if (AuthorizeService.GetAccessSettings(HttpContext).SettingsOther != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке изменить параметры прочих настроек. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\settings");
                    }
                    if (otherSettings is null || !ModelState.IsValid)
                    {
                        _logger.LogError($"Ошибка при попытке сохранить прочие настройки. Неверные аргументы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке сохранить прочие настройки. Неверные аргументы.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    ConnectorCenterApp.Instance.OtherSettings = otherSettings;
                    SettingsConfigurationService.SaveConfiguration(ConnectorCenterApp.Instance.OtherSettings);
                    _logger.LogInformation($"Изменены прочие настройки.");
                    return View("Index",
                        new IndexViewModel(
                            AuthorizeService.GetAccessSettings(HttpContext),
                            AuthorizeService.GetUserRole(HttpContext)
                        ));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при попытке сохранить прочие настройки. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке сохранить прочие настройки.",
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
        public async Task<IActionResult> SaveMyUserSettings(UserSettings? userSettings)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    if (userSettings is null || !ModelState.IsValid || userSettings.Id == 0 || userSettings.AppUserId == 0)
                    {
                        _logger.LogError($"Ошибка при попытке сохранить личные настройки. Неверные аргументы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке сохранить личные настройки. Неверные аргументы.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"Назад",@"\settings\my" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    
                    userSettings.AppUserId = AuthorizeService.GetUserId(HttpContext);
                    _context.Update(userSettings);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Личные настройки пользователя успешно изменены.");
                    return View("Index",
                        new IndexViewModel(
                            AuthorizeService.GetAccessSettings(HttpContext),
                            AuthorizeService.GetUserRole(HttpContext)
                        ));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при попытке сохранить личные настройки. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке сохранить личные настройки.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"Назад",@"\settings\my" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveMyColorScheme(ColorScheme? colorScheme)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    if (colorScheme is null || !ModelState.IsValid || colorScheme.Id == 0)
                    {
                        _logger.LogError($"Ошибка при попытке сохранить цветовую схему. Неверные аргументы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке сохранить цветовую схему. Неверные аргументы.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"Назад",@"\settings\my" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    _context.Update(colorScheme);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Цветовая схема пользователя успешно сохранена.");
                    return View("Index",
                        new IndexViewModel(
                            AuthorizeService.GetAccessSettings(HttpContext),
                            AuthorizeService.GetUserRole(HttpContext)
                        ));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при попытке сохранить цветовую схему. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке сохранить цветовую схему.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"Назад",@"\settings\my" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> DefaultMyColorScheme(long Id)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    ColorScheme? currentColorScheme = _context.ColorSchemes.Find(Id);
                    if (currentColorScheme is null)
                    {
                        _logger.LogWarning($"Ошибка при попытке сбросить цветовую схему. Не найдено в БД. Будет установлено по умолчанию");
                        currentColorScheme = ColorScheme.GetDefault();
                    }
                    else
                    {
                        ColorScheme defaultColorScheme = ColorScheme.GetDefault();
                        currentColorScheme.Fone = defaultColorScheme.Fone;
                        currentColorScheme.Accent = defaultColorScheme.Accent;
                        currentColorScheme.SubAccent = defaultColorScheme.SubAccent;
                        currentColorScheme.Panel = defaultColorScheme.Panel;
                        currentColorScheme.Border = defaultColorScheme.Border;
                        currentColorScheme.Path = defaultColorScheme.Path;
                        currentColorScheme.Text = defaultColorScheme.Text;
                        currentColorScheme.Select = defaultColorScheme.Select;
                        currentColorScheme.Error = defaultColorScheme.Error;
                        currentColorScheme.Disable = defaultColorScheme.Disable;
                    }                    
                    _context.Update(currentColorScheme);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Визуальные настройки пользователя успешно сброшены.");
                    return View("Index",
                        new IndexViewModel(
                            AuthorizeService.GetAccessSettings(HttpContext),
                            AuthorizeService.GetUserRole(HttpContext)
                        ));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при попытке сбросить цветовую схему. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке сбросить цветовую схему.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"Назад",@"\settings\my" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveMyFontScheme(FontScheme? fontScheme)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    if (fontScheme is null || !ModelState.IsValid || fontScheme.Id == 0)
                    {
                        _logger.LogError($"Ошибка при попытке сохранить схему шрифтов. Неверные аргументы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке сохранить схему . Неверные аргументы.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"Назад",@"\settings\my" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    _context.Update(fontScheme);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Cхема шрифтов пользователя успешно сохранена.");
                    return View("Index",
                        new IndexViewModel(
                            AuthorizeService.GetAccessSettings(HttpContext),
                            AuthorizeService.GetUserRole(HttpContext)
                        ));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при попытке сохранить схему шрифтов. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке сохранить схему шрифтов.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"Назад",@"\settings\my" },
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
