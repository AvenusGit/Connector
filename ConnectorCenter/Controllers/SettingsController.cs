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

                    List<AppUser> users = await _context.Users
                       .Include(user => user.UserSettings)
                       .Include(user => user.VisualScheme)
                           .ThenInclude(vs => vs.ColorScheme)
                                   .ThenInclude(cs => cs.Fone)
                       .Include(user => user.VisualScheme)
                           .ThenInclude(vs => vs.ColorScheme)
                               .ThenInclude(cs => cs.Accent)
                       .Include(user => user.VisualScheme)
                           .ThenInclude(vs => vs.ColorScheme)
                                   .ThenInclude(cs => cs.SubAccent)                       
                       .Include(user => user.VisualScheme)
                           .ThenInclude(vs => vs.ColorScheme)
                                   .ThenInclude(cs => cs.Panel)
                       .Include(user => user.VisualScheme)
                           .ThenInclude(vs => vs.ColorScheme)
                                   .ThenInclude(cs => cs.Border)
                       .Include(user => user.VisualScheme)
                           .ThenInclude(vs => vs.ColorScheme)
                                   .ThenInclude(cs => cs.Path)
                        .Include(user => user.VisualScheme)
                           .ThenInclude(vs => vs.ColorScheme)
                                   .ThenInclude(cs => cs.Text)
                        .Include(user => user.VisualScheme)
                           .ThenInclude(vs => vs.ColorScheme)
                                   .ThenInclude(cs => cs.Select)
                        .Include(user => user.VisualScheme)
                           .ThenInclude(vs => vs.ColorScheme)
                                   .ThenInclude(cs => cs.Error)
                        .Include(user => user.VisualScheme)
                           .ThenInclude(vs => vs.ColorScheme)
                                   .ThenInclude(cs => cs.Disable)
                        .Include(user => user.VisualScheme)
                            .ThenInclude(vs => vs.FontScheme)
                       .Where(user => user.Id == AuthorizeService.GetUserId(HttpContext))
                       .ToListAsync();

                    AppUser? currentUser = users.FirstOrDefault(user => user.Id == AuthorizeService.GetUserId(HttpContext));


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
                    //_context.Users
                    //    .Where(user => user.Id == AuthorizeService.GetUserId(HttpContext))
                    //    .Load();
                    AppUser? user = await _context.Users
                        .Where(user => user.Id == AuthorizeService.GetUserId(HttpContext))
                        .FirstOrDefaultAsync();
                    user.VisualScheme.ColorScheme = colorScheme;
                    _context.Update(user);
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
        #endregion

    }
}
