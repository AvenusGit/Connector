using Microsoft.AspNetCore.Mvc;
using ConnectorCenter.Views.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using ConnectorCenter.Data;
using Microsoft.EntityFrameworkCore;
using ConnectorCenter.Models.Settings;
using ConnectorCenter.Services.Authorize;

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
                try
                {
                    _logger.LogInformation($"Запрошена страница меню настроек.");
                    return View("Index");
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
                try
                {
                    _logger.LogInformation($"Запрошена страница настроек доступа.");
                    //TODO only admin acess there
                    return View(new AccessModel(ConnectorCenterApp.Instance.SupportAccessSettings));
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
        #endregion
        #region POST
        [HttpPost]
        public IActionResult SaveSupportAccessSettings(AccessSettings? supportAccessSettings)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    if(supportAccessSettings is null || !ModelState.IsValid)
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
                    _logger.LogInformation($"Изменены настройки доступа техподдержки.");
                    return View("Index");                    
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
        #endregion
    }
}
