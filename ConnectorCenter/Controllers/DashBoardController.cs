using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ConnectorCenter.Data;
using ConnectorCenter.Views.FirstStart;
using ConnectorCore.Models;
using ConnectorCenter.Services.Authorize;

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
                try
                {
                    _logger.LogInformation($"Запрос страницы главного меню.");
                    return View();
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
