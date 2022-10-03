using Microsoft.AspNetCore.Mvc;
using ConnectorCore.Models;
using ConnectorCenter.Models.Settings;
using ConnectorCenter.Services.Configurations;
using ConnectorCenter.Services.Archive;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using ConnectorCenter.Services.Authorize;
using ConnectorCenter.Data;
using Microsoft.EntityFrameworkCore;
using ConnectorCenter.Services.Archive;

namespace ConnectorCenter.Controllers
{
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class ExportController : Controller
    {
        #region Fields
        private readonly ILogger _logger;
        private DataBaseContext _context;
        #endregion
        public ExportController(ILogger<AppUserGroupsController> logger, DataBaseContext context)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult UserAccessSettings()
        {
            using (var scope = _logger.BeginScope($"FILE({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetUserRole(HttpContext) == AppUser.AppRoles.Administrator)
                    {
                        _logger.LogInformation($"Запрошен экспорт настроек доступа роли пользователь.");
                        return SendFile(
                            Encoding.UTF8.GetBytes(SettingsConfigurationService
                                            .XmlSerialize(ConnectorCenterApp.Instance.UserAccessSettings)),
                                            "UserAccessSettings.conf");
                    }
                    else
                    {
                        _logger.LogWarning($"Отказано в запросе экспорта настроек доступа роли пользователь. У роли пользователя нет доступа.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\ie");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе экспорта настроек доступа роли пользователь. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе экспорта настроек доступа роли пользователь.",
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
        [HttpGet]
        public IActionResult SupportAccessSettings()
        {
            using (var scope = _logger.BeginScope($"FILE({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetUserRole(HttpContext) == AppUser.AppRoles.Administrator)
                    {
                        _logger.LogInformation($"Запрошен экспорт настроек доступа роли поддержка.");
                        return SendFile(
                            Encoding.UTF8.GetBytes(SettingsConfigurationService
                                            .XmlSerialize(ConnectorCenterApp.Instance.SupportAccessSettings))
                                    , "SupportAccessSettings.conf");
                    }
                    else
                    {
                        _logger.LogWarning($"Отказано в запросе экспорта настроек доступа роли поддержка. У роли пользователя нет доступа.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\ie");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе экспорта настроек доступа роли поддержка. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе экспорта настроек доступа роли поддержка.",
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
        [HttpGet]
        public IActionResult ApiSettings()
        {
            using (var scope = _logger.BeginScope($"FILE({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetAccessSettings(HttpContext).SettingsAPI != AccessSettings.AccessModes.None)
                    {
                        _logger.LogInformation($"Запрошен экспорт настроек API.");
                        return SendFile(
                            Encoding.UTF8.GetBytes(SettingsConfigurationService
                                            .XmlSerialize(ConnectorCenterApp.Instance.ApiSettings))
                                    , "ApiSettings.conf");
                    }
                    else
                    {
                        _logger.LogWarning($"Отказано в запросе экспорта настроек API. У роли пользователя нет доступа.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\ie");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе экспорта настроек API. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе экспорта настроек API.",
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
        [HttpGet]
        public IActionResult LogSettings()
        {
            using (var scope = _logger.BeginScope($"FILE({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetAccessSettings(HttpContext).SettingsLogs != AccessSettings.AccessModes.None)
                    {
                        _logger.LogInformation($"Запрошен экспорт настроек логов.");
                        return SendFile(
                            Encoding.UTF8.GetBytes(ConnectorCenterApp.Instance.LogSettings.ToString())
                                    , "LogSettings.conf");
                    }
                    else
                    {
                        _logger.LogWarning($"Отказано в запросе экспорта настроек логов. У роли пользователя нет доступа.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\ie");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе экспорта настроек логов. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе экспорта настроек логов.",
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
        [HttpGet]
        public IActionResult OtherSettings()
        {
            using (var scope = _logger.BeginScope($"FILE({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetAccessSettings(HttpContext).SettingsOther != AccessSettings.AccessModes.None)
                    {
                        _logger.LogInformation($"Запрошен экспорт прочих настроек.");
                        return SendFile(
                            Encoding.UTF8.GetBytes(SettingsConfigurationService
                                            .XmlSerialize(ConnectorCenterApp.Instance.OtherSettings))
                                    , "OtherSettings.conf");
                    }
                    else
                    {
                        _logger.LogWarning($"Отказано в запросе экспорта прочих настроек. У роли пользователя нет доступа.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\ie");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе экспорта прочих настроек. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе экспорта прочих настроек.",
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
        [HttpGet]
        public async Task<IActionResult> MySettings()
        {
            using (var scope = _logger.BeginScope($"FILE({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();                    
                    long userId = AuthorizeService.GetUserId(HttpContext);

                    UserSettings? mySettings = await _context.UserSettings
                        .FirstOrDefaultAsync(us => us.AppUserId == userId);
                    if(mySettings is not null)
                    {
                        _logger.LogInformation($"Запрошен экспорт личных настроек.");
                        return SendFile(
                        Encoding.UTF8.GetBytes(SettingsConfigurationService
                                        .XmlSerialize(mySettings))
                                , "MySettings.conf");
                    }
                    else
                    {
                        mySettings = UserSettings.GetDefault();
                        _context.Update(mySettings);
                        _context.SaveChanges();
                        _logger.LogWarning($"Ошибка при запросе экспорта личных настроек. У пользователя не найдены настройки. Будут назначены по умолчанию.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе экспорта личных настроек. Настройки не найдены. Назначены по умолчанию.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"Назад",@"\settings\ie" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 500
                            }));
                    }                                     
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе экспорта личных настроек. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе экспорта личных настроек.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"Назад",@"\ie" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }
            }
        }
        [HttpGet]
        public IActionResult AllSettings()
        {
            using (var scope = _logger.BeginScope($"FILE({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetUserRole(HttpContext) == AppUser.AppRoles.Administrator)
                    {
                        _logger.LogInformation($"Запрошен экспорт всех настроек.");
                        return SendFile(
                            ArchiveService.GetArchive(
                                new Services.Archive.File[]
                                {
                                    new Services.Archive.File()
                                    {
                                        Name = "UserAccessSettings.conf",
                                        Data = Encoding.UTF8.GetBytes(SettingsConfigurationService
                                            .XmlSerialize(ConnectorCenterApp.Instance.UserAccessSettings))
                                    },
                                    new Services.Archive.File()
                                    {
                                        Name = "SupportAccessSettings.conf",
                                        Data = Encoding.UTF8.GetBytes(SettingsConfigurationService
                                            .XmlSerialize(ConnectorCenterApp.Instance.SupportAccessSettings))
                                    },
                                    new Services.Archive.File()
                                    {
                                        Name = "OtherSettings.conf",
                                        Data = Encoding.UTF8.GetBytes(SettingsConfigurationService
                                            .XmlSerialize(ConnectorCenterApp.Instance.OtherSettings))
                                    },
                                    new Services.Archive.File()
                                    {
                                        Name = "ApiSettings.conf",
                                        Data = Encoding.UTF8.GetBytes(SettingsConfigurationService
                                            .XmlSerialize(ConnectorCenterApp.Instance.ApiSettings))
                                    },
                                    new Services.Archive.File()
                                    {
                                        Name = "LogSettings.conf",
                                        Data =  Encoding.UTF8.GetBytes(ConnectorCenterApp.Instance.LogSettings.ToString())
                                    }
                                })
                            ,"ConnectorCenterSettings.zip");
                        //return SendFile(
                        //    Encoding.UTF8.GetBytes(SettingsConfigurationService
                        //                    .XmlSerialize(ConnectorCenterApp.Instance.OtherSettings))
                        //            , "OtherSettings.conf");
                    }
                    else
                    {
                        _logger.LogWarning($"Отказано в запросе экспорта всех настроек. У роли пользователя нет доступа.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\ie");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе экспорта всех настроек. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе экспорта прочих настроек.",
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
        #region Methods
        private FileResult SendFile(byte[] file, string fileName)
        {
            return File(file, "application/x-compressed", fileName);
        }
        #endregion
    }
}
