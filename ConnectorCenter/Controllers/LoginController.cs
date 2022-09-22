using Microsoft.AspNetCore.Mvc;
using ConnectorCenter.Views.Login;
using Microsoft.AspNetCore.Authorization;
using ConnectorCore.Models;
using ConnectorCenter.Data;
using ConnectorCenter.Services.Authorize;

namespace ConnectorCenter.Controllers
{
    /// <summary>
    /// Контроллер авторизации и аутентификации через куки
    /// </summary>
    [AllowAnonymous]
    public class LoginController : Controller
    {
        #region Fields
        private readonly ILogger<HomeController> _logger;
        private readonly DataBaseContext _dataBaseContext;
        #endregion
        #region Constructors
        public LoginController(ILogger<HomeController> logger, DataBaseContext context)
        {
            _logger = logger;
            _dataBaseContext = context;
        }
        #endregion
        #region GET
        [HttpGet]
        public IActionResult Index(string? message)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    _logger.LogInformation($"Запрос страницы авторизации.");
                    return View("Index",new IndexModel(HttpContext.User.Identity?.Name, message));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы авторизации. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы авторизации.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"Повторить запрос",@"\login" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }                
            }
        }
        #endregion
        #region POST
        /// <summary>
        /// Запрос на авторизацию в приложении через Cookies
        /// </summary>
        /// <param name="сredentials">Учетные данные для авторизации</param>
        /// <returns>Переадресовывает либо в меню, либо на страницу первого запуска либо на страницы авторизации в зависимости от результата.</returns>
        [HttpPost]
        public IActionResult Authorize(Сredentials сredentials)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    if (!ModelState.IsValid)
                    {
                        _logger.LogWarning($"Ошибка при попытке авторизации в приложении через Cookies. Неверные аргументы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке авторизации в приложении через Cookies. Неверные аргументы.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"К авторизации",@"\login" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    AppUser? user;
                    if (AuthorizeService.IsAuthorized(_dataBaseContext, сredentials, out user))
                    {
                        if( !AuthorizeService.GetAccessSettings(user.Role).WebAccess)
                        {
                            _logger.LogWarning($"Ошибка при попытке авторизации в приложении. Роли пользователя не разрешен веб доступ.");
                            return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                new
                                {
                                    message = "Ошибка при попытке авторизации в приложении. Роли пользователя не разрешен веб доступ.",
                                    buttons = new Dictionary<string, string>()
                                    {
                                        {"Повторная авторизация",@"\login" }                                    
                                    },
                                    errorCode = 403
                                }));
                        }
                        if(!user!.IsEnabled)
                        {
                            _logger.LogError($"Ошибка при попытке авторизации в приложении. Пользователь деактивирован.");
                            return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                new
                                {
                                    message = "Ошибка при попытке авторизации в приложении. Пользователь деактивирован.",
                                    buttons = new Dictionary<string, string>()
                                    {
                                        {"Повторная авторизация",@"\login" }
                                    },
                                    errorCode = 403
                                }));
                        }

                        CookieAuthorizeService.SignIn(HttpContext, user);
                        _logger.LogInformation($"Авторизация пройдена успешно. Пользователь - {user.Name}.");
                        if (user.Credentials.IsIdentical(AppUser.GetDefault().Credentials))
                        {
                            _logger.LogInformation($"Авторизация пройдена под временным пользователем. Будет выполнена переадресация на страницу " +
                                $"первого запуска.");
                            return RedirectToAction("Index", "FirstStart");
                        }                            
                        return RedirectToAction("Index", "DashBoard");
                    }
                    else
                    {
                        _logger.LogWarning($"Ошибка авторизации. Неверный логин/пароль.");
                        return View("Index", new IndexModel(null,"Неверный логин/пароль"));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при попытке авторизации в приложении через Cookies. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке авторизации в приложении через Cookies.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"К авторизации",@"\login" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }                
            }
        }
        /// <summary>
        /// Запрос на выход из аккаунта (удаление куков)
        /// </summary>
        /// <returns>Страница авторизации</returns>
        [HttpPost]
        public new IActionResult SignOut()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    CookieAuthorizeService.SignOut(HttpContext);
                    _logger.LogInformation($"Выполнен выход из аккаунта пользователя {AuthorizeService.GetUserName(HttpContext)}.");
                    return View("Index", new IndexModel(null, "Выполнен выход из аккаунта"));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе на выход из аккаунта. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе на выход из аккаунта.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"К авторизации",@"\login" },
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
