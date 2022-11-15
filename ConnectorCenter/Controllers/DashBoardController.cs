using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ConnectorCenter.Data;
using ConnectorCenter.Views.FirstStart;
using ConnectorCenter.Views.DashBoard;
using ConnectorCore.Models;
using ConnectorCenter.Services.Authorize;
using ConnectorCenter.Models.Settings;

namespace ConnectorCenter.Controllers
{
    /// <summary>
    /// Контроллер главного меню
    /// </summary>
    [Authorize(Policy = "Cookies")]
    public class DashBoardController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataBaseContext _dataBaseContext;

        public DashBoardController(ILogger<HomeController> logger, DataBaseContext context)
        {
            _logger = logger;
            _dataBaseContext = context;
        }
        public IActionResult Index()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    _logger.LogInformation($"Запрос страницы главного меню.");
                    return View(new Views.DashBoard.IndexViewModel(AuthorizeService.GetAccessSettings(HttpContext),
                        AuthorizeService.GetUserName(HttpContext), AuthorizeService.GetUserRole(HttpContext)));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы главного меню. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы главного меню.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"Повторить попытку",@"\dashboard" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }                
            }
        }
    }
}
