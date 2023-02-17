using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConnectorCenter.Data;
using ConnectorCenter.Views.AppUsers;
using ConnectorCore.Models;
using ConnectorCore.Models.VisualModels;
using ConnectorCenter.Services.Authorize;
using ConnectorCore.Models.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ConnectorCenter.Models.Settings;
using ConnectorCore.Cryptography;

namespace ConnectorCenter.Controllers
{
    /// <summary>
    /// AppUser controller
    /// </summary>
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class AppUsersController : Controller
    {
        #region Fields
        /// <summary>
        /// Current database context
        /// </summary>
        private readonly DataBaseContext _context;
        /// <summary>
        /// Current logger
        /// </summary>
        private readonly ILogger _logger;
        #endregion
        #region Constructors
        public AppUsersController(DataBaseContext context, ILogger<AppUserGroupsController> logger)
        {
            _context = context;
            _logger = logger;
        }
        #endregion
        #region GET
        /// <summary>
        /// GET: user list request
        /// </summary>
        /// <returns>User list page</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    AccessSettings currentAcessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (currentAcessSettings.Users == AccessSettings.AccessModes.None)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу списка пользователей. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\dashboard");
                    }
                    _logger.LogInformation("Запрос на получение списка пользователей");
                    return _context.Users is null
                        ? View(new IndexModel(new List<AppUser>(), currentAcessSettings))
                        : View(new IndexModel(await _context.Users
                            .Include(usr => usr.Groups)
                                .ThenInclude(gr => gr.Connections)
                            .Include(user => user.Credentials)
                            .Include(user => user.Connections)
                            .ToListAsync(), currentAcessSettings));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы списка пользователей. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Внутренняя ошибка обработки запроса страницы списка пользователей.",
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
        /// <summary>
        /// GET: request page for adding anew user
        /// </summary>
        /// <returns>Page for adding a new user</returns>
        [HttpGet]
        public IActionResult Add()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetAccessSettings(HttpContext).Users != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу добавления пользователя. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUsers");
                    }
                    _logger.LogInformation("Запрос страницы добавления пользователя");
                    return View(new AddModel());
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы добавления пользователя. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Внутренняя ошибка обработки запроса страницы добавления пользователя.",
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
        /// <summary>
        /// GET: page request to edit user
        /// </summary>
        /// <param name="id">User identifier</param>
        /// <returns>User's edit page</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(long? id)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    AccessSettings currentAcessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (currentAcessSettings.Users != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу изменения пользователя. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUsers");
                    }
                    if (id == null)
                    {
                        _logger.LogWarning($"Ошибка при запросе страницы редактирования пользователя. Нет идентификатора или контекста.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе страницы редактирования пользователя. Нет идентификатора или контекста.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }

                    AppUser? appUser = await _context.Users.Include(usr => usr.Credentials).Where(usr => usr.Id == id).FirstOrDefaultAsync();
                    if (appUser == null)
                    {
                        _logger.LogWarning($"Ошибка при запросе страницы редактирования пользователя. Пользователь не найден.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе страницы редактирования пользователя. Пользователь не найден.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }

                    appUser.Credentials.Password = "֍password֍";
                    _logger.LogInformation($"Запрос страницы для редактирования пользователя {appUser.Name}.");
                    return View(new EditModel(appUser, currentAcessSettings));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы редатирования пользователя. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы редатирования пользователя.",
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
        /// <summary>
        /// GET: request connection list for user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>Connection list page</returns>
        [HttpGet]
        public async Task<IActionResult> ShowConnections(long? userId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    AccessSettings currentAcessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (currentAcessSettings.UserConnections == AccessSettings.AccessModes.None)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу подключений пользователя. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUsers");
                    }
                    if (!userId.HasValue)
                    {
                        _logger.LogWarning($"Ошибка при запросе страницы подключений пользователя. Нет идентификатора.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе страницы подключений пользователя. Нет идентификатора.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    AppUser? user = await _context.Users
                        .Include(usr => usr.Groups)
                            .ThenInclude(gr => gr.Connections)
                                .ThenInclude(conn => conn.ServerUser)
                                    .ThenInclude(usr => usr!.Credentials)
                        .Include(usr => usr.Groups)
                            .ThenInclude(gr => gr.Connections)
                                .ThenInclude(conn => conn.Server)
                        .Include(usr => usr.Connections)
                            .ThenInclude(conn => conn.ServerUser)
                                .ThenInclude(usr => usr!.Credentials)
                        .Include(usr => usr.Connections)
                            .ThenInclude(conn => conn.Server)
                        .FirstOrDefaultAsync(usr => usr.Id == userId);
                    if (user != null)
                        return View(new ShowConnectionsModel(user, currentAcessSettings));
                    else
                    {
                        _logger.LogWarning($"Ошибка при запросе страницы подключений пользователя. Пользователь не найден.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе страницы подключений пользователя. Пользователь не найден.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы подключений пользователя. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы подключений пользователя.",
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
        /// <summary>
        /// GET: page request to adding connection
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>Add connection page </returns>
        [HttpGet]
        public async Task<IActionResult> AddConnections(long? userId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    AccessSettings currentAcessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (currentAcessSettings.UserConnections != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу добавления подключения для пользователя. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUsers");
                    }
                    if (!userId.HasValue)
                    {
                        _logger.LogWarning($"Ошибка при запросе страницы добавления подключений пользователю. Нет идентификатора.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе страницы добавления подключений пользователю. Нет идентификатора.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    AppUser? user = await _context.Users
                        .Include(usr => usr.Groups)
                            .ThenInclude(gr => gr.Connections)
                                .ThenInclude(conn => conn.ServerUser)
                                    .ThenInclude(usr => usr!.Credentials)
                        .Include(usr => usr.Groups)
                            .ThenInclude(gr => gr.Connections)
                                .ThenInclude(conn => conn.Server)
                        .Include(usr => usr.Connections)
                                .ThenInclude(conn => conn.Server)
                        .FirstOrDefaultAsync(usr => usr.Id == userId);
                    List<Server> servers = _context.Servers
                        .Include(srv => srv.Connections)
                            .ThenInclude(conn => conn.ServerUser)
                                .ThenInclude(usr => usr!.Credentials)
                        .ToList();
                    if (user is null)
                    {
                        _logger.LogWarning($"Ошибка при запросе страницы добавления подключений пользователю. Пользователь не найден.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе страницы добавления подключений пользователю. Пользователь не найден.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                    if (servers is null)
                        servers = new List<Server>();
                    _logger.LogInformation($"Запрос на страницу доабвления подключений пользователю.");
                    return View(new AddConnectionsModel(servers, user));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы добавления подключений пользователю. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы добавления подключений пользователю.",
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
        /// <summary>
        /// POST: request adding new user
        /// </summary>
        /// <param name="appUser">New user</param>
        /// <returns>User's list page</returns>
        [HttpPost]
        public async Task<IActionResult> Add(AppUser appUser)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetAccessSettings(HttpContext).Users != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке добавить пользователя. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUsers");
                    }
                    if (ModelState.IsValid)
                    {
                        if (String.IsNullOrEmpty(appUser.Credentials.Login) || String.IsNullOrEmpty(appUser.Credentials.Password))
                        {
                            _logger.LogWarning("Отказано в попытке добавления пользователя. Не указан логин или пароль.");
                            return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                new
                                {
                                    message = "Отказано в попытке добавления пользователя. Не указан логин или пароль.",
                                    buttons = new Dictionary<string, string>()
                                    {
                                        {"На главную",@"\dashboard" },
                                        {"К логам",@"\logs" }
                                    },
                                    errorCode = 400
                                }));
                        }
                        if (AppUserExist(appUser.Credentials.Login))
                        {
                            _logger.LogWarning("Отказано в попытке добавления пользователя. Пользователь с таким логином уже существует.");
                            return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                new
                                {
                                    message = "Пользователь с таким логином уже существует",
                                    buttons = new Dictionary<string, string>()
                                    {
                                        {"На главную",@"\dashboard" },
                                        {"К логам",@"\logs" }
                                    },
                                    errorCode = 400
                                }));
                        }

                        appUser.Credentials.Password = PasswordCryptography.GetUserPasswordHash(
                            appUser.Credentials.Login,
                            appUser.Credentials.Password
                            );

                        appUser.VisualScheme = VisualScheme.GetDefaultVisualScheme();
                        _context.Users.Add(appUser);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"Добавлен новый пользователь {appUser.Name} под логином {appUser.Credentials?.Login}");
                        return RedirectToAction("Index", "AppUsers");
                    }
                    _logger.LogError($"Внутренняя ошибка обработки запроса на добавление пользователя. Неверные аргументы pfghjcf.");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Внутренняя ошибка обработки запроса на добавление пользователя. Неверные аргументы.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 400
                        }));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Внутренняя ошибка обработки запроса на добавление пользователя. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Внутренняя ошибка обработки запроса на добавление пользователя.",
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
        /// <summary>
        /// POST: user editing request
        /// </summary>
        /// <param name="appUser">User for editing</param>
        /// <returns>User's list page</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(AppUser appUser)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetAccessSettings(HttpContext).Users != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу изменения пользователя. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUsers");
                    }
                    if (ModelState.IsValid)
                    {
                        if (String.IsNullOrEmpty(appUser.Credentials.Login) || String.IsNullOrEmpty(appUser.Credentials.Password))
                        {
                            _logger.LogWarning("Отказано в попытке изменения пользователя. Логин или пароль пусты.");
                            return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                new
                                {
                                    message = "Отказано в попытке добавления пользователя. Логин или пароль пусты.",
                                    buttons = new Dictionary<string, string>()
                                    {
                                        {"На главную",@"\dashboard" },
                                        {"К логам",@"\logs" }
                                    },
                                    errorCode = 400
                                }));
                        }
                        AppUser? user = _context.Users
                            .Include(usr => usr.Credentials)
                            .FirstOrDefault(x => x.Id == appUser.Id);
                        if (user is not null)
                        {
                            user.Name = appUser.Name;
                            user.Credentials.Login = appUser.Credentials.Login;
                            if (appUser.Credentials.Password != "֍password֍")
                                user.Credentials.Password = PasswordCryptography.GetUserPasswordHash(
                                    appUser.Credentials.Login,
                                    appUser.Credentials.Password);                       
                            user.Role = appUser.Role;
                            user.IsEnabled = appUser.IsEnabled;
                            _context.Update(user);
                            await _context.SaveChangesAsync();
                            _logger.LogInformation($"Изменен пользователь {appUser.Name} (ID:{appUser.Id}).");
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            _logger.LogWarning($"Ошибка при изменении пользователя. Пользователь не найден.");
                            return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                new
                                {
                                    message = "Ошибка при изменении пользователя. Пользователь не найден.",
                                    buttons = new Dictionary<string, string>()
                                    {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                                    },
                                    errorCode = 404
                                }));
                        }
                    }
                    _logger.LogWarning($"Ошибка при изменении пользователя. Неверные аргументы.");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при изменении пользователя. Неверные аргументы.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 400
                        }));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при изменении пользователя. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при изменении пользователя.",
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
        /// <summary>
        /// POST: user delete request
        /// </summary>
        /// <param name="id">User identifier</param>
        /// <returns>Users list page</returns>
        [HttpPost]
        public async Task<IActionResult> Delete(long? id)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetAccessSettings(HttpContext).Users != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу удаления пользователя. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUsers");
                    }
                    if (id == null)
                    {
                        _logger.LogWarning($"Ошибка при запросе удаления пользователя. Не указан идентификатор.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе удаления пользователя. Не указан идентификатор.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }

                    AppUser? appUser = await _context.Users
                        .Include(usr => usr.Connections)
                        .FirstOrDefaultAsync(m => m.Id == id);
                    if (appUser != null)
                    {
                        if(appUser.Role == AppUser.AppRoles.Administrator && 
                            await _context.Users
                                .Where(usr => usr.Role == AppUser.AppRoles.Administrator && usr.Id != appUser.Id)
                                .AnyAsync()
                          )
                        {
                            appUser.Connections.Clear(); // очистка подключений, чтобы они не были каскадно удалены с пользователем
                            _context.Update(appUser);
                            await _context.SaveChangesAsync();
                            _context.Users.Remove(appUser);
                            await _context.SaveChangesAsync();
                            if (AuthorizeService.CompareHttpUserWithAppUser(HttpContext.User, appUser))
                            {
                                CookieAuthorizeService.SignOut(HttpContext);
                                return RedirectToAction("Index", "Login", new RouteValueDictionary(
                                            new { message = "Ваш пользователь удален." }));
                            }
                            _logger.LogInformation($"Удален пользователь {appUser.Name}.");
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            _logger.LogWarning($"Отказано в попытке удалить единственного администратора.");
                            return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                new
                                {
                                    message = "Запрещено удалять единственного администратора. Создайте другого администратора перед удалением.",
                                    buttons = new Dictionary<string, string>()
                                    {
                                        {"На главную",@"\dashboard" },
                                        {"К логам",@"\logs" },
                                        {"К пользователям",@"\appUsers" }
                                    },
                                    errorCode = 412
                                }));
                        }                        
                    }
                    else
                    {
                        _logger.LogWarning($"Ошибка при запросе удаления пользователя. Пользователь не найден.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе удаления пользователя. Пользователь не найден.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе удаления пользователя. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе удаления пользователя.",
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
        /// <summary>
        /// POST: change enable mode
        /// </summary>
        /// <param name="id">User identifier</param>
        /// <returns>User list page</returns>
        [HttpPost]
        public async Task<IActionResult> ChangeEnableMode(long? id)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetAccessSettings(HttpContext).Users != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке изменить статус активности пользователя. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUsers");
                    }
                    if (id == null)
                    {
                        _logger.LogError($"Ошибка при запросе изменения активности пользователя. Нет идентификатора.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе изменения активности пользователя. Нет идентификатора.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    AppUser? user = await _context.Users.FindAsync(id) ?? null!;
                    if (user == null)
                    {
                        _logger.LogError($"Ошибка при запросе изменения активности пользователя. Пользователь не найден.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе изменения активности пользователя. Пользователь не найден.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                    user.IsEnabled = !user.IsEnabled;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Изменен статус активности пользователя {user.Name} с {!user.IsEnabled} на {user.IsEnabled}.");
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе изменения активности пользователя. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе изменения активности пользователя.",
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

        /// <summary>
        /// POST: add connection to user request
        /// </summary>
        /// <param name="connectionId">Connection identifier</param>
        /// <param name="userId">User identifier</param>
        /// <returns>User connection list page</returns>
        [HttpPost]
        public async Task<IActionResult> AddConnections(long? connectionId, long? userId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetAccessSettings(HttpContext).UserConnections != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке добавить подключение для пользователя. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUsers");
                    }
                    if (!connectionId.HasValue || !userId.HasValue)
                    {
                        _logger.LogWarning($"Ошибка при запросе добавления подключения пользователю. Неверные аргументы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе добавления подключения пользователю. Неверные аргументы.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    AppUser? user = await _context.Users
                        .Include(usr => usr.Groups)
                            .ThenInclude(gr => gr.Connections)
                                .ThenInclude(conn => conn.ServerUser)
                                    .ThenInclude(usr => usr!.Credentials)

                        .Include(usr => usr.Connections)
                        .FirstOrDefaultAsync(usr => usr.Id == userId);
                    if (user is null)
                    {
                        _logger.LogWarning($"Ошибка при запросе добавления подключения пользователю. Пользователь не найден.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе добавления подключения пользователю. Пользователь не найден.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                    Connection? connection = await _context.Connections.FindAsync(connectionId);
                    if (connection is null)
                    {
                        _logger.LogWarning($"Ошибка при запросе добавления подключения пользователю. Подключение не найдено.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе добавления подключения пользователю. Подключение не найдено.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                    user.Connections.Add(connection);
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    List<Server> servers = _context.Servers
                        .Include(srv => srv.Connections)
                            .ThenInclude(conn => conn.ServerUser)
                                .ThenInclude(usr => usr!.Credentials)
                        .ToList();
                    _logger.LogInformation($"Подключение {connection.ConnectionName} добавлено пользователю {user.Name}.");
                    return View("AddConnections", new AddConnectionsModel(servers, user));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе добавления подключения пользователю. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе добавления подключения пользователю.",
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

        /// <summary>
        /// POST: Remove connection from user connection list request - add connection page version
        /// </summary>
        /// <param name="connectionId">Connection identifier</param>
        /// <param name="userId">User identifier</param>
        /// <returns>Add connection list page</returns>
        [HttpPost]
        public async Task<IActionResult> DropConnectionOnAddConnectionList(long? connectionId, long? userId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetAccessSettings(HttpContext).UserConnections != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке удалить подключение для пользователя. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUsers");
                    }
                    if (!connectionId.HasValue || !userId.HasValue)
                    {
                        _logger.LogWarning($"Ошибка при запросе удаления подключения у пользователя. Неверные аргументы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе удаления подключения у пользователя. Неверные аргументы.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }

                    AppUser? user = await _context.Users
                        .Include(usr => usr.Groups)
                            .ThenInclude(gr => gr.Connections)
                                .ThenInclude(conn => conn.ServerUser)
                                    .ThenInclude(usr => usr!.Credentials)
                        .Include(usr => usr.Groups)
                            .ThenInclude(gr => gr.Connections)
                                .ThenInclude(conn => conn.Server)
                        .Include(usr => usr.Connections)
                            .ThenInclude(conn => conn.ServerUser)
                                .ThenInclude(usr => usr!.Credentials)
                        .Include(usr => usr.Connections)
                            .ThenInclude(conn => conn.Server)
                        .FirstOrDefaultAsync(usr => usr.Id == userId);

                    if (user is null)
                    {
                        _logger.LogWarning($"Ошибка при запросе удаления подключения у пользователя. Пользователь не найден.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе удаления подключения у пользователя. Пользователь не найден.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }

                    await DropConnection(connectionId.Value, user);
                    List<Server> servers = _context.Servers
                        .Include(srv => srv.Connections)
                            .ThenInclude(conn => conn.ServerUser)
                                .ThenInclude(usr => usr!.Credentials)
                        .ToList();
                    return View("AddConnections", new AddConnectionsModel(servers, user));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе удаления подключения у пользователя. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе удаления подключения у пользователя.",
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

        /// <summary>
        /// POST: Remove connection from user connection list request - connection list page version
        /// </summary>
        /// <param name="connectionId">Connection identifier</param>
        /// <param name="userId">User identifier</param>
        /// <returns>Connection list page</returns>
        [HttpPost]
        public async Task<IActionResult> DropConnectionOnConnectionList(long? connectionId, long? userId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    AccessSettings currentAcessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (currentAcessSettings.UserConnections != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке удалить подключение для пользователя. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUsers");
                    }
                    if (!connectionId.HasValue || !userId.HasValue)
                    {
                        _logger.LogWarning($"Ошибка при запросе удаления подключения у пользователя. Неверные аргументы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе удаления подключения у пользователя. Неверные аргументы.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    AppUser? user = await _context.Users
                        .Include(usr => usr.Groups)
                            .ThenInclude(gr => gr.Connections)
                                .ThenInclude(conn => conn.ServerUser)
                                    .ThenInclude(usr => usr!.Credentials)
                        .Include(usr => usr.Groups)
                            .ThenInclude(gr => gr.Connections)
                                .ThenInclude(conn => conn.Server)
                        .Include(usr => usr.Connections)
                            .ThenInclude(conn => conn.ServerUser)
                                .ThenInclude(usr => usr!.Credentials)
                        .Include(usr => usr.Connections)
                            .ThenInclude(conn => conn.Server)
                        .FirstOrDefaultAsync(usr => usr.Id == userId);

                    if (user is null)
                    {
                        _logger.LogWarning($"Ошибка при запросе удаления подключения у пользователя. Пользователь не найден.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе удаления подключения у пользователя. Пользователь не найден.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                    await DropConnection(connectionId.Value, user);
                    return View("ShowConnections", new ShowConnectionsModel(user, currentAcessSettings));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе удаления подключения у пользователя. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе удаления подключения у пользователя.",
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

        /// <summary>
        /// POST: Set to default selected user visual settings
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>User list page</returns>
        [HttpPost]
        public async Task<IActionResult> DropUserVisualSetting(long? userId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetAccessSettings(HttpContext).ResetVisualSettings != true)
                    {
                        _logger.LogWarning("Отказано в попытке сбросить визуальные настройки пользователя. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUsers");
                    }

                    AppUser? user = await _context.Users
                        .Include(usr => usr.VisualScheme)
                        .FirstOrDefaultAsync(usr => usr.Id == userId);

                    if (user is null)
                    {
                        _logger.LogWarning($"Ошибка при запросе сброса визуальных настроек у пользователя. Пользователь не найден.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе сброса визуальных настроек у пользователя. Пользователь не найден.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                    user.VisualScheme = VisualScheme.GetDefaultVisualScheme();
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Сброшены визуальные настройки у пользователя {user.Name}.");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = $"У пользователя {user.Name} успешно сброшены визуальные настройки.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"К списку",@"\appUsers" },
                                {"К логам",@"\logs" }
                            }
                            
                        }));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе сброса визуальных настроек у пользователя. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе сброса визуальных настроек у пользователя.",
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
        #region Methods
        /// <summary>
        /// Remove connection on selected user
        /// </summary>
        /// <param name="connectionId">Connection identifier</param>
        /// <param name="user">Selected user instance</param>
        private async Task DropConnection(long connectionId, AppUser user)
        {
            Connection? connection = await _context.Connections.FindAsync(connectionId);
            if (connection is null)
            {
                _logger.LogError($"Ошибка при запросе удаления подключения у пользователя. Подключение не найдено.");
                return;
            }
            user.Connections.Remove(connection);
            _context.Update(user);
            _logger.LogInformation($"Подключение {connection.ConnectionName} удалено у пользователя {user.Name}.");
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// CHeck on exist user in DB by identifier
        /// </summary>
        /// <param name="id">User identifier</param>
        /// <returns>True - user exist</returns>
        private bool AppUserExists(long id)
        {
            return (_context.Users?
                .Include(usr => usr.UserSettings)
                .Any(e => e.Id == id)).GetValueOrDefault();
        }
        /// <summary>
        /// CHeck on exist user in DB by login name
        /// </summary>
        /// <param name="login">User login (AppUser.Credentials.Login)</param>
        /// <returns>True - user exist</returns>
        private bool AppUserExist(string login)
        {
            return (_context.Users?.Any(usr => usr.Credentials.Login == login)).GetValueOrDefault();
        }
        #endregion
    }
}
