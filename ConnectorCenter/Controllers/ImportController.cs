using ConnectorCenter.Data;
using ConnectorCenter.Models.Settings;
using ConnectorCenter.Services.Authorize;
using ConnectorCenter.Services.Configurations;
using ConnectorCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ConnectorCenter.Controllers
{
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class ImportController : Controller
    {
        #region Fields
        private readonly ILogger _logger;
        private DataBaseContext _context;
        #endregion
        public ImportController(ILogger<AppUserGroupsController> logger, DataBaseContext context)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult UserAccessSettings(IFormFile? file)
        {
            using (var scope = _logger.BeginScope($"FILE({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetUserRole(HttpContext) == AppUser.AppRoles.Administrator)
                    {
                        _logger.LogInformation($"Запрошен импорт настроек доступа роли пользователь.");
                        if (file is not null || file?.Length == 0)
                        {
                            var conf = GetConfigurationFromFile<AccessSettings>(file);
                            if(conf is AccessSettings)
                            {
                                ConnectorCenterApp.Instance.UserAccessSettings = (AccessSettings)conf;
                                SettingsConfigurationService.SaveConfiguration(ConnectorCenterApp.Instance.UserAccessSettings);
                                _logger.LogInformation($"Импорт настроек доступа роли пользователь прошел успешно.");
                                return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                    new
                                    {
                                        message = "Импорт настроек доступа роли пользователь прошел успешно.",
                                        buttons = new Dictionary<string, string>()
                                        {
                                            {"Ок",@"\settings\ie" },
                                            {"К логам",@"\logs" }
                                        },
                                        errorCode = 200
                                    }));
                            }
                            else
                            {
                                _logger.LogError($"Ошибка при запросе импорта настроек доступа роли пользователь. Файл не распознан как настройка доступа.");
                                return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                    new
                                    {
                                        message = "Ошибка при запросе импорта настроек доступа роли пользователь. Файл не распознан.",
                                        buttons = new Dictionary<string, string>()
                                        {
                                    {"Назад",@"\settings\ie" },
                                    {"К логам",@"\logs" }
                                        },
                                        errorCode = 400
                                    }));
                            }
                        }
                        else
                        {
                            _logger.LogError($"Ошибка при запросе импорта настроек доступа роли пользователь. Файл не указан или пуст.");
                            return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                new
                                {
                                    message = "Ошибка при запросе импорта настроек доступа роли пользователь. Файл не указан или пуст.",
                                    buttons = new Dictionary<string, string>()
                                    {
                                        {"Назад",@"\settings\ie" },
                                        {"К логам",@"\logs" }
                                    },
                                    errorCode = 400
                                }));
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Отказано в запросе импорта настроек доступа роли пользователь. У роли пользователя нет доступа.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\ie");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе импорта настроек доступа роли пользователь. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе импорта настроек доступа роли пользователь.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"Назад",@"\settings\ie" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }
            }
        }
        [HttpPost]
        public IActionResult SupportAccessSettings(IFormFile? file)
        {
            using (var scope = _logger.BeginScope($"FILE({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetUserRole(HttpContext) == AppUser.AppRoles.Administrator)
                    {
                        _logger.LogInformation($"Запрошен импорт настроек доступа роли поддержка.");
                        if (file is not null || file?.Length == 0)
                        {
                            var conf = GetConfigurationFromFile<AccessSettings>(file);
                            if (conf is AccessSettings)
                            {
                                ConnectorCenterApp.Instance.SupportAccessSettings = (AccessSettings)conf;
                                SettingsConfigurationService.SaveConfiguration(ConnectorCenterApp.Instance.SupportAccessSettings);
                                _logger.LogInformation($"Импорт настроек доступа роли поддержка прошел успешно.");
                                return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                    new
                                    {
                                        message = "Импорт настроек доступа роли поддержка прошел успешно.",
                                        buttons = new Dictionary<string, string>()
                                        {
                                            {"Ок",@"\settings\ie" },
                                            {"К логам",@"\logs" }
                                        },
                                        errorCode = 200
                                    }));
                            }
                            else
                            {
                                _logger.LogError($"Ошибка при запросе импорта настроек доступа роли поддержка. Файл не распознан как настройка доступа.");
                                return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                    new
                                    {
                                        message = "Ошибка при запросе импорта настроек доступа роли поддержка. Файл не распознан.",
                                        buttons = new Dictionary<string, string>()
                                        {
                                    {"Назад",@"\settings\ie" },
                                    {"К логам",@"\logs" }
                                        },
                                        errorCode = 400
                                    }));
                            }
                        }
                        else
                        {
                            _logger.LogError($"Ошибка при запросе импорта настроек доступа роли поддержка. Файл не указан или пуст.");
                            return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                new
                                {
                                    message = "Ошибка при запросе импорта настроек доступа роли поддержка. Файл не указан или пуст.",
                                    buttons = new Dictionary<string, string>()
                                    {
                                        {"Назад",@"\settings\ie" },
                                        {"К логам",@"\logs" }
                                    },
                                    errorCode = 400
                                }));
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Отказано в запросе импорта настроек доступа роли поддержка. У роли пользователя нет доступа.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\ie");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе импорта настроек доступа роли поддержка. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе импорта настроек доступа роли поддержка.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"Назад",@"\settings\ie" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }
            }
        }
        [HttpPost]
        public IActionResult ApiSettings(IFormFile? file)
        {
            using (var scope = _logger.BeginScope($"FILE({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetAccessSettings(HttpContext)?.SettingsAPI == AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogInformation($"Запрошен импорт настроек API.");
                        if (file is not null || file?.Length == 0)
                        {
                            var conf = GetConfigurationFromFile<ApiSettings>(file);
                            if (conf is ApiSettings)
                            {
                                ConnectorCenterApp.Instance.ApiSettings = (ApiSettings)conf;
                                SettingsConfigurationService.SaveConfiguration(ConnectorCenterApp.Instance.ApiSettings);
                                _logger.LogInformation($"Импорт настроек API прошел успешно.");
                                return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                    new
                                    {
                                        message = "Импорт настроек API прошел успешно.",
                                        buttons = new Dictionary<string, string>()
                                        {
                                            {"Ок",@"\settings\ie" },
                                            {"К логам",@"\logs" }
                                        },
                                        errorCode = 200
                                    }));
                            }
                            else
                            {
                                _logger.LogError($"Ошибка при запросе импорта настроек API. Файл не распознан как настройка доступа.");
                                return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                    new
                                    {
                                        message = "Ошибка при запросе импорта настроек API. Файл не распознан.",
                                        buttons = new Dictionary<string, string>()
                                        {
                                    {"Назад",@"\settings\ie" },
                                    {"К логам",@"\logs" }
                                        },
                                        errorCode = 400
                                    }));
                            }
                        }
                        else
                        {
                            _logger.LogError($"Ошибка при запросе импорта настроек API. Файл не указан или пуст.");
                            return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                new
                                {
                                    message = "Ошибка при запросе импорта настроек API. Файл не указан или пуст.",
                                    buttons = new Dictionary<string, string>()
                                    {
                                        {"Назад",@"\settings\ie" },
                                        {"К логам",@"\logs" }
                                    },
                                    errorCode = 400
                                }));
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Отказано в запросе импорта настроек API. У роли пользователя нет доступа.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\ie");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе импорта настроек API. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе импорта настроек API.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"Назад",@"\settings\ie" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }
            }
        }
        [HttpPost]
        public IActionResult LogSettings(IFormFile? file)
        {
            using (var scope = _logger.BeginScope($"FILE({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetAccessSettings(HttpContext)?.SettingsLogs == AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogInformation($"Запрошен импорт настроек логов.");
                        if (file is not null || file?.Length == 0)
                        {
                            string confString;
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                file.CopyTo(memoryStream);
                                byte[] fileBytes = memoryStream.ToArray();
                                confString = Encoding.UTF8.GetString(fileBytes);
                            }
                            LogSettings? conf = Models.Settings.LogSettings.LoadConfiguration(confString);
                            if (conf is LogSettings)
                            {
                                ConnectorCenterApp.Instance.LogSettings = (LogSettings)conf;
                                ConnectorCenterApp.Instance.LogSettings.SaveConfiguration();
                                _logger.LogInformation($"Импорт настроек логов прошел успешно.");
                                return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                    new
                                    {
                                        message = "Импорт настроек логов прошел успешно.",
                                        buttons = new Dictionary<string, string>()
                                        {
                                            {"Ок",@"\settings\ie" },
                                            {"К логам",@"\logs" }
                                        },
                                        errorCode = 200
                                    }));
                            }
                            else
                            {
                                _logger.LogError($"Ошибка при запросе импорта настроек логов. Файл не распознан как настройка доступа.");
                                return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                    new
                                    {
                                        message = "Ошибка при запросе импорта настроек логов. Файл не распознан.",
                                        buttons = new Dictionary<string, string>()
                                        {
                                    {"Назад",@"\settings\ie" },
                                    {"К логам",@"\logs" }
                                        },
                                        errorCode = 400
                                    }));
                            }
                        }
                        else
                        {
                            _logger.LogError($"Ошибка при запросе импорта настроек логов. Файл не указан или пуст.");
                            return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                new
                                {
                                    message = "Ошибка при запросе импорта настроек логов. Файл не указан или пуст.",
                                    buttons = new Dictionary<string, string>()
                                    {
                                        {"Назад",@"\settings\ie" },
                                        {"К логам",@"\logs" }
                                    },
                                    errorCode = 400
                                }));
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Отказано в запросе импорта настроек логов. У роли пользователя нет доступа.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\ie");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе импорта настроек логов. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе импорта настроек логов.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"Назад",@"\settings\ie" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }
            }
        }
        [HttpPost]
        public IActionResult OtherSettings(IFormFile? file)
        {
            using (var scope = _logger.BeginScope($"FILE({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetAccessSettings(HttpContext)?.SettingsOther == AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogInformation($"Запрошен импорт прочих настроек.");
                        if (file is not null || file?.Length == 0)
                        {
                            var conf = GetConfigurationFromFile<OtherSettings>(file);
                            if (conf is OtherSettings)
                            {
                                ConnectorCenterApp.Instance.OtherSettings = (OtherSettings)conf;
                                SettingsConfigurationService.SaveConfiguration(ConnectorCenterApp.Instance.OtherSettings);
                                _logger.LogInformation($"Импорт прочих настроек прошел успешно.");
                                return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                    new
                                    {
                                        message = "Импорт прочих настроек прошел успешно.",
                                        buttons = new Dictionary<string, string>()
                                        {
                                            {"Ок",@"\settings\ie" },
                                            {"К логам",@"\logs" }
                                        },
                                        errorCode = 200
                                    }));
                            }
                            else
                            {
                                _logger.LogError($"Ошибка при запросе импорта прочих настроек. Файл не распознан как настройка доступа.");
                                return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                    new
                                    {
                                        message = "Ошибка при запросе импорта прочих настроек. Файл не распознан.",
                                        buttons = new Dictionary<string, string>()
                                        {
                                    {"Назад",@"\settings\ie" },
                                    {"К логам",@"\logs" }
                                        },
                                        errorCode = 400
                                    }));
                            }
                        }
                        else
                        {
                            _logger.LogError($"Ошибка при запросе импорта прочих настроек. Файл не указан или пуст.");
                            return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                new
                                {
                                    message = "Ошибка при запросе импорта прочих настроек. Файл не указан или пуст.",
                                    buttons = new Dictionary<string, string>()
                                    {
                                        {"Назад",@"\settings\ie" },
                                        {"К логам",@"\logs" }
                                    },
                                    errorCode = 400
                                }));
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Отказано в запросе импорта прочих настроек. У роли пользователя нет доступа.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\ie");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе импорта прочих настроек. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе импорта прочих настроек.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"Назад",@"\settings\ie" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> MySettings(IFormFile? file)
        {
            using (var scope = _logger.BeginScope($"FILE({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    _logger.LogInformation($"Запрошен импорт личных настроек.");
                    if (file is not null || file?.Length == 0)
                    {
                        var conf = GetConfigurationFromFile<UserSettings>(file);
                        if (conf is UserSettings)
                        {
                            AppUser? user = await _context.Users
                                .FindAsync(AuthorizeService.GetUserId(HttpContext));
                            if (user != null)
                            {
                                user.UserSettings = (UserSettings)conf;
                                _context.Update(user);
                                _logger.LogInformation($"Импорт личных настроек пользователю {user.Name} прошел успешно.");
                                return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                    new
                                    {
                                        message = $"Импорт личных настроек пользователю {user.Name} прошел успешно.",
                                        buttons = new Dictionary<string, string>()
                                        {
                                            {"Ок",@"\settings\ie" },
                                            {"К логам",@"\logs" }
                                        },
                                        errorCode = 200
                                    }));
                            }
                            else
                            {
                                _logger.LogError($"Ошибка при запросе импорта личных настроек. Пользователь не найден в БД.");
                                return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                    new
                                    {
                                        message = "Ошибка при запросе импорта личных настроек. Пользователь не найден в БД.",
                                        buttons = new Dictionary<string, string>()
                                        {
                                            {"Назад",@"\settings\ie" },
                                            {"К логам",@"\logs" }
                                        },
                                        errorCode = 400
                                    }));
                            }
                            
                        }
                        else
                        {
                            _logger.LogError($"Ошибка при запросе импорта личных настроек. Файл не распознан как настройка доступа.");
                            return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                new
                                {
                                    message = "Ошибка при запросе импорта личных настроек. Файл не распознан.",
                                    buttons = new Dictionary<string, string>()
                                    {
                                {"Назад",@"\settings\ie" },
                                {"К логам",@"\logs" }
                                    },
                                    errorCode = 400
                                }));
                        }
                    }
                    else
                    {
                        _logger.LogError($"Ошибка при запросе импорта личных настроек. Файл не указан или пуст.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе импорта личных настроек. Файл не указан или пуст.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"Назад",@"\settings\ie" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе импорта личных настроек. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе импорта личных настроек.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"Назад",@"\settings\ie" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }
            }
        }

        private ConfigurationType? GetConfigurationFromFile<ConfigurationType>(IFormFile file)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();
                string result = Encoding.UTF8.GetString(fileBytes);
                return SettingsConfigurationService.XmlDeserialize<ConfigurationType>(result);
            }
        }
    }
}
