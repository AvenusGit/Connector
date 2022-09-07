using ConnectorCenter.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ConnectorCenter.Views.Login;
using ConnectorCenter.Services.Authorize;
using Microsoft.AspNetCore.Authorization;

namespace ConnectorCenter.Controllers
{
    /// <summary>
    /// Контроллер корня приложения
    /// </summary>
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// GET запрос корня приложения
        /// </summary>
        /// <returns>Если пользователь аутентифицирован - страница меню, иначе - страница авторизации.</returns>
        public IActionResult Index()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    if (HttpContext.User.Identity != null)
                    {
                        if(HttpContext.User.Identity.IsAuthenticated)
                        {
                            _logger.LogInformation($"При запросе корня приложения пользователь признан аутентифицированным." +
                                $" Выполнена переадресация на страницу меню.");
                            return RedirectToAction("Index", "DashBoard");
                        }                            
                    }
                    _logger.LogInformation($"При запросе корня приложения пользователь признан не аутентифицированным." +
                                $" Выполнена переадресация на страницу авторизации.");
                    return RedirectToAction("Index", "Login");
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе корня приложения. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе корня приложения.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"Повторить запрос",@"\home" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }                
            }
        }
    }
}