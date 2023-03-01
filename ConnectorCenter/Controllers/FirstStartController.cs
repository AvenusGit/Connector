using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ConnectorCenter.Data;
using ConnectorCenter.Services.Authorize;
using ConnectorCenter.Views.FirstStart;
using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using ConnectorCore.Models.VisualModels.Interfaces;
using ConnectorCore.Models.VisualModels;
using ConnectorCenter.Models.Settings;
using ConnectorCore.Cryptography;
using ConnectorCenter.Models.Repository;

namespace ConnectorCenter.Controllers
{
    /// <summary>
    /// Контроллер для работы с первым запуском приложения
    /// </summary>
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class FirstStartController : Controller
    {
        #region Fields
        private readonly ILogger<HomeController> _logger;
        private readonly AppUserRepository _appUserRepository;
        #endregion
        #region Constructors
        public FirstStartController(ILogger<HomeController> logger, AppUserRepository appUserRepository)
        {
            _logger = logger;
            _appUserRepository = appUserRepository;
        }
        #endregion
        #region GET
        /// <summary>
        /// Запрос страницы первого запуска приложения
        /// </summary>
        /// <returns>Страница первого запуска приложения</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    _logger.LogInformation($"Запрос страницы первого запуска.");
                    bool isFirstStart = await _appUserRepository.Count() < 1;
                    return View(new IndexViewModel(isFirstStart));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы первого запуска. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы первого запуска.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"Повторить запрос",@"\firstStart" },
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
        public async Task<IActionResult> CreateFirstUser(string username, Credentials credentials)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        _logger.LogWarning($"Ошибка при запросе сохранении первого пользователя. Неверные аргументы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе сохранении первого пользователя. Неверные аргументы.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"Повторить попытку",@"\firstStart" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    if(String.IsNullOrEmpty(credentials.Login) || String.IsNullOrEmpty(credentials.Password))
                    {
                        _logger.LogWarning($"Ошибка при запросе сохранении первого пользователя. Логин или пароль пусты.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе сохранении первого пользователя. Логин или пароль пусты.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"Повторить попытку",@"\firstStart" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    AppUser newUser = new AppUser()
                    {
                        Name = username,
                        IsEnabled = true,
                        Credentials = credentials,
                        Role = AppUser.AppRoles.Administrator,
                        Connections = new List<Connection>(),
                        UserSettings = UserSettings.GetDefault(),
                        VisualScheme = VisualScheme.GetDefaultVisualScheme()
                    };
                    newUser.Credentials.Password = PasswordCryptography.GetUserPasswordHash(
                        newUser.Credentials.Login,
                        newUser.Credentials.Password);
                    await _appUserRepository.Add(newUser);
                    _logger.LogInformation($"Выполнено сохранение первого пользователя - {newUser.Name}. Назначены права администратора.");
                    CookieAuthorizeService.SignOut(HttpContext);
                    _logger.LogInformation($"Будет выполнен выход из под временного пользователя ввиду наличия постоянных.");
                    return RedirectToAction("Index", "Login");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе сохранении первого пользователя. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе сохранении первого пользователя.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"Повторить попытку",@"\firstStart" },
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
