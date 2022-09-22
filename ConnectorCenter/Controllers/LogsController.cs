using Microsoft.AspNetCore.Mvc;
using ConnectorCenter.Views.Logs;
using System.IO;
using System.Text;
using ConnectorCenter.Services.Authorize;
using ConnectorCenter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ConnectorCenter.Services.Logs;
using Microsoft.AspNetCore.Authorization;
using ConnectorCenter.Models.Settings;

namespace ConnectorCenter.Controllers
{
    /// <summary>
    /// Контроллер для показа логов приложения
    /// </summary>
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class LogsController : Controller
    {
        #region Fields
        private readonly ILogger _logger;
        #endregion
        #region Constructors
        public LogsController(ILogger<AppUserGroupsController> logger)
        {
            _logger = logger;
        }
        #endregion
        #region GET
        /// <summary>
        /// Запрос на просмотр логов в веб форме
        /// </summary>
        /// <returns>Страница с текстом лога</returns>
        [HttpGet]
        public IActionResult Index()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (!accessSettings.Logs)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу с логами. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\dashboard");
                    }
                    _logger.LogInformation("Запрошены логи в WEB форме.");
                    return View(new IndexModel(System.IO.File.ReadAllText(LogService.GetLastLogFilePath())));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при попытке показать логи в веб форме - {ex.Message}.{ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке показать логи.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"На главную",@"\dashboard" },
                                {"Повторить попытку",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }
            }
        }
        #endregion
    }
}
