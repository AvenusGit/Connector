using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConnectorCenter.Data;
using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using ConnectorCenter.Views.AppUserGroups;
using Microsoft.AspNetCore.Authorization;
using ConnectorCenter.Services.Authorize;
using ConnectorCenter.Models.Settings;

namespace ConnectorCenter.Controllers
{
    /// <summary>
    /// Контроллер для работы с группами пользователей
    /// </summary>
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class AppUserGroupsController : Controller
    {
        #region Fields
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly DataBaseContext _context;
        #endregion
        #region Constructors
        public AppUserGroupsController(DataBaseContext context, ILogger<AppUserGroupsController> logger)
        {
            _context = context;
            _logger = logger;
        }
        #endregion
        #region GET
        /// <summary>
        /// Get: Запрос страницы групп пользователей.
        /// </summary>
        /// <returns>Страница групп пользователей</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if(accessSettings.Groups == AccessSettings.AccessModes.None)
                    {
                        _logger.LogWarning("Отказано в попытке запросить список групп. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\dashboard");
                    }
                    _logger.LogInformation("Запрошен список групп пользователей.");
                    return _context.UserGroups is null
                          ? View(new IndexModel(new List<AppUserGroup>(), accessSettings))
                          : View(new IndexModel(await _context.UserGroups
                                .Include(gr => gr.Connections)
                                .Include(gr => gr.Users)
                                .ToListAsync(), accessSettings));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе списка групп пользователей. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Внутренняя ошибка обработки запроса страницы групп пользователей.",
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
        /// Get: Запрос страницы добавления группы пользователей
        /// </summary>
        /// <returns>Страница добавления группы пользователей</returns>
        [HttpGet]
        public IActionResult Add()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.Groups != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу добавления групп. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUserGroups");
                    }
                    _logger.LogInformation("Запрошена страница добавления группы пользователей.");
                    return View(new AddAppUserGroupsModel());
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы добавления группы пользователей. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Внутренняя ошибка обработки запроса страницы добавления группы пользователей.",
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
        /// Запрос страницы редактирования группы
        /// </summary>
        /// <param name="id">Идентификатор группы для редактирования</param>
        /// <returns>Возврат на страницу групп</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(long? id)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.Groups != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу изменения группы. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUserGroups");
                    }
                    if (!id.HasValue)
                    {
                        _logger.LogWarning($"Ошибка  при запросе страницы изменения группы пользователей. Не указан ИД группы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке получить страницу изменения группы пользователей. Не указан ИД.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }

                    AppUserGroup? appUserGroup = await _context.UserGroups.FindAsync(id);
                    if (appUserGroup == null)
                    {
                        _logger.LogWarning($"Ошибка  при запросе страницы изменения группы пользователей. Не найдена группа в БД.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Не найдена группа в БД при попытке получить страницу изменения группы пользователей.",
                                buttons = new Dictionary<string, string>()
                                {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                    _logger.LogInformation($"Запрошена страница изменения группы пользователей {appUserGroup.GroupName}."); ;
                    return View(new EditUserGroupsModel(appUserGroup));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка  при запросе страницы изменения группы пользователей. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке получить страницу изменения группы пользователей.",
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
        /// Запрос страницы подключений указанной группы
        /// </summary>
        /// <param name="groupId">Идентификатор группы</param>
        /// <returns>Страница подключений указанной группы</returns>
        [HttpGet]
        public async Task<IActionResult> ShowConnections(long? groupId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.GroupConnections == AccessSettings.AccessModes.None)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу списка подключений группы. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUserGroups");
                    }
                    if (!groupId.HasValue)
                    {
                        _logger.LogWarning($"Ошибка при запросе страницы подключений группы пользователей. Не указан ИД группы."); ;
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                new
                                {
                                    message = "Ошибка при запросе страницы подключений группы пользователей. Не указан ИД группы.",
                                    buttons = new Dictionary<string, string>()
                                    {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                    },
                                    errorCode = 400
                                }));
                    }

                    AppUserGroup? userGroup = await _context.UserGroups
                        .Include(usr => usr.Connections)
                            .ThenInclude(conn => conn.ServerUser)
                                .ThenInclude(usr => usr!.Credentials)
                        .FirstOrDefaultAsync(usr => usr.Id == groupId);
                    if (userGroup != null)
                    {
                        _logger.LogInformation($"Запрошены подключения группы пользователей:{userGroup.GroupName}."); ;
                        return View(new ShowConnectionsModel(userGroup, accessSettings));
                    }
                    _logger.LogWarning($"Не найдена запрошенная группа пользователей:{userGroup.GroupName}."); ;
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы подключений группы пользователей. Не найдена группа в БД.",
                            buttons = new Dictionary<string, string>()
                            {
                            {"На главную",@"\dashboard" },
                            {"К логам",@"\logs" }
                            },
                            errorCode = 404
                        }));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе подключений группы пользователей. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе подключений группы пользователей.",
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
        /// Запрос страницы подключений группы пользователей
        /// </summary>
        /// <param name="groupId">Идентификатор группы пользователей</param>
        /// <returns>Страница подключений указанной группы</returns>
        [HttpGet]
        public async Task<IActionResult> AddConnections(long? groupId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.GroupConnections != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке запросить добавления подключений группы. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUserGroups\showConnections");
                    }
                    if (!groupId.HasValue)
                    {
                        _logger.LogError($"Ошибка при запросе страницы добавления подключений для группы пользователей. Не указан ИД группы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Не указан ИД при запросе страницы добавления подключений.",
                                buttons = new Dictionary<string, string>()
                                {
                            {"На главную",@"\dashboard" },
                            {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    AppUserGroup? group = await _context.UserGroups.FindAsync(groupId);
                    List<Server> servers = _context.Servers
                        .Include(srv => srv.Connections)
                            .ThenInclude(conn => conn.ServerUser)
                                .ThenInclude(usr => usr!.Credentials)
                        .ToList();
                    if (group is null)
                    {
                        _logger.LogError($"Ошибка при запросе страницы добавления подключений для группы пользователей. Группа не найдена в БД.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "В БД не найдена группа при запросе страницы добавления подключений.",
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
                    _logger.LogInformation($"Запрошена страница добавления подключений к группе {group.GroupName}.");
                    return View(new AddConnectionsModel(servers, group));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы подключений группы пользователей. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы подключений группы пользователей.",
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
        /// Запрос на страницу списка пользователей группы
        /// </summary>
        /// <param name="groupId">Идентификатор группы</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ShowUsers(long? groupId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.GroupUsers == AccessSettings.AccessModes.None)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу списка пользователей группы. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUserGroups");
                    }
                    if (!groupId.HasValue)
                    {
                        _logger.LogWarning($"Ошибка при попытке просмотреть список пользователей. Не указан идентификатор группы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке попытке просмотреть список пользователей.",
                                buttons = new Dictionary<string, string>()
                                {
                            {"На главную",@"\dashboard" },
                            {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    AppUserGroup? userGroup = await _context.UserGroups
                        .Include(gr => gr.Users)
                            .ThenInclude(usr => usr.Credentials)
                        .FirstOrDefaultAsync(usr => usr.Id == groupId);
                    if (userGroup != null)
                    {
                        _logger.LogInformation($"Запрос на страницу пользователй группы {userGroup.GroupName}");
                        return View(new ShowUsersModel(userGroup, accessSettings));
                    }
                    else
                    {
                        _logger.LogWarning($"Ошибка при попытке просмотреть список пользователей. Не найдена группа.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке попытке просмотреть список пользователей. Не найдена группа.",
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
                    _logger.LogError($"Ошибка при попытке просмотреть список пользователей. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке попытке просмотреть список пользователей.",
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
        /// Запрос страницы добавления пользователя в группу
        /// </summary>
        /// <param name="groupId">Идентификатор группы</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AddUser(long? groupId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.GroupUsers != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу добавления пользователей в группу. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUserGroups\showConnections");
                    }
                    if (!groupId.HasValue)
                    {
                        _logger.LogError($"Ошибка при попытке получить страницу добавления пользователя в группу. Нет идентификатора группы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Не получилось добавить пользователя в группу. Нет идентификатора.",
                                buttons = new Dictionary<string, string>()
                                {
                            {"На главную",@"\dashboard" },
                            {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    AppUserGroup? group = await _context.UserGroups
                        .Include(gr => gr.Users)
                        .FirstOrDefaultAsync(gr => gr.Id == groupId);
                    List<AppUser> users = _context.Users
                        .Include(usr => usr.Credentials)
                        .ToList();
                    if (group is null)
                    {
                        _logger.LogError($"Ошибка при попытке получить страницу добавления пользователя в группу. Группа не найдена.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Не получилось добавить пользователя в группу. Группы нет в БД..",
                                buttons = new Dictionary<string, string>()
                                {
                            {"На главную",@"\dashboard" },
                            {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                    if (users is null)
                        users = new List<AppUser>();
                    _logger.LogInformation("Запрос страницы добавления пользователя в группу.");
                    return View(new AddUserModel(users, group));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при попытке получить страницу добавления пользователя в группу. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Не получилось добавить пользователя в группу.",
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
        /// POST: Запрос на добавление новой группы
        /// </summary>
        /// <param name="appUserGroup">Новая группа</param>
        /// <returns>Возвращает в список групп</returns>
        [HttpPost]
        public async Task<IActionResult> Add(AppUserGroup appUserGroup)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.Groups != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке добавления группы. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUserGroups");
                    }
                    if (ModelState.IsValid)
                    {
                        _context.UserGroups.Add(appUserGroup);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"Добавлена новая группа пользователей:{appUserGroup.GroupName}."); ;
                        return RedirectToAction("Index");
                    }
                    _logger.LogError("Ошибка проверки модели при попытке добавить группу пользователей.");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке добавить новую группу пользователей - неверные аргументы.",
                            buttons = new Dictionary<string, string>()
                            {
                            {"На главную",@"\dashboard" },
                            {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка  при попытке добавить группу пользователей. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке добавить новую группу пользователей.",
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
        /// Запрос на изменение группы
        /// </summary>
        /// <param name="appUserGroup">Группа для изменения</param>
        /// <returns>Возврат на страницу групп</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(AppUserGroup appUserGroup)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.Groups != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке изменения группы. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUserGroups");
                    }
                    if (ModelState.IsValid)
                    {
                        _context.Update(appUserGroup);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"Изменена группа пользователей:{appUserGroup.GroupName}."); ;
                        return RedirectToAction("Index");
                    }
                    _logger.LogWarning("Ошибка проверки модели при попытке изменить группу пользователей.");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке изменить группу пользователей - неверные аргументы.",
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
                    _logger.LogError($"Ошибка при попытке сохранить изменения группы пользователей. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке сохранить изменения группы пользователей.",
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
        /// Запрос на удаление группы
        /// </summary>
        /// <param name="id">Идентификатор группы</param>
        /// <returns>Возврат на страницу групп</returns>
        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.Groups != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке удаления группы. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUserGroups");
                    }
                    AppUserGroup? appUserGroup = await _context.UserGroups
                        .Include(gr => gr.Users)
                        .Include(gr => gr.Connections)
                        .FirstOrDefaultAsync(usr => usr.Id == id);
                    if (appUserGroup != null)
                    {
                        appUserGroup.Connections.Clear();
                        appUserGroup.Users.Clear(); // очистка пользователей и подключений, чтобы они не были удалены каскадно
                        _context.Update(appUserGroup);
                        await _context.SaveChangesAsync();
                        _context.UserGroups.Remove(appUserGroup);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"Удалена группа пользователей:{appUserGroup.GroupName}."); ;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        _logger.LogWarning($"Ошибка при попытке удалить группу пользователей - требуемая группа не найдена в БД.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке удалить группу пользователей. Группа не найдена в БД.",
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
                    _logger.LogError($"Ошибка при попытке удалить группу пользователей. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке удалить группу пользователей.",
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
        /// Запрос на добавление подключения к группе
        /// </summary>
        /// <param name="connectionId">Идентификатор подключения</param>
        /// <param name="groupId">Идентификатор группы</param>
        /// <returns>Возврат на страницу добавления подключений</returns>
        [HttpPost]
        public async Task<IActionResult> AddConnections(long? connectionId, long? groupId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.GroupConnections != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке добавления подключения группе. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUserGroups\showConnections");
                    }
                    if (!connectionId.HasValue || !groupId.HasValue)
                    {
                        _logger.LogError($"Ошибка при попытке добавить подключение группе пользователей. Не указаны идентификаторы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке добавить подключение группе пользователей.  Не указаны идентификаторы.",
                                buttons = new Dictionary<string, string>()
                                {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                                },
                                errorCode = 500
                            }));
                    }
                    AppUserGroup? group = await _context.UserGroups
                        .Include(usr => usr.Connections)
                        .Where(usr => usr.Id == groupId)
                        .FirstOrDefaultAsync();
                    Connection? connection = await _context.Connections.FindAsync(connectionId);
                    if (connection is null)
                    {
                        _logger.LogWarning($"Ошибка при попытке добавить подключение группе пользователей. Не найдено подключение.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке добавить подключение группе пользователей. Не найдено подключение.",
                                buttons = new Dictionary<string, string>()
                                {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                    group.Connections.Add(connection);
                    _context.Update(group);
                    await _context.SaveChangesAsync();
                    List<Server> servers = _context.Servers
                        .Include(srv => srv.Connections)
                            .ThenInclude(conn => conn.ServerUser)
                                .ThenInclude(usr => usr!.Credentials)
                        .ToList();
                    _logger.LogInformation($"Добавлено подключение {connection.ConnectionName} к группе {group.GroupName}.");
                    return View("AddConnections", new AddConnectionsModel(servers, group));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при попытке добавить подключение группе пользователей. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке добавить подключение группе пользователей.",
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
        /// Запрос на удаление подключения у группы из списка добавлений подключений
        /// </summary>
        /// <param name="connectionId">Идентификатор подключения</param>
        /// <param name="groupId">Идентификатор группы</param>
        /// <returns>Страница добавления подключений</returns>
        [HttpPost]
        public async Task<IActionResult> DropConnectionOnAddConnectionList(long? connectionId, long? groupId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.GroupConnections != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке удаления подключения из группы. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUserGroups\showConnections");
                    }
                    if (!connectionId.HasValue || !groupId.HasValue)
                    {
                        _logger.LogWarning($"Ошибка при попытке удалить подключение у группы. Не указаны идентификаторы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке попытке удалить подключение у группы. Нет идентификаторов.",
                                buttons = new Dictionary<string, string>()
                                {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }

                    AppUserGroup? group = await _context.UserGroups
                        .Include(usr => usr.Connections)
                        .Where(usr => usr.Id == groupId)
                        .FirstOrDefaultAsync();

                    if (group is null)
                    {
                        _logger.LogWarning($"Ошибка при попытке удалить подключение у группы. Не найдена группа.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке попытке удалить подключение у группы. Не найдена группа.",
                                buttons = new Dictionary<string, string>()
                                {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }

                    await DropConnection(connectionId.Value, group);
                    List<Server> servers = _context.Servers
                        .Include(srv => srv.Connections)
                            .ThenInclude(conn => conn.ServerUser)
                                .ThenInclude(usr => usr!.Credentials)
                        .ToList();
                    return View("AddConnections", new AddConnectionsModel(servers, group));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при попытке удалить подключение у группы. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке попытке удалить подключение у группы.",
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
        /// Запрос на удаление подключения у группы из списка подключений
        /// </summary>
        /// <param name="connectionId">Идентификатор подключения</param>
        /// <param name="groupId">Идентификатор группы</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DropConnectionOnConnectionList(long? connectionId, long? groupId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.GroupConnections != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке удаления подключения из группы. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUserGroups\showConnections");
                    }
                    if (!connectionId.HasValue || !groupId.HasValue)
                    {
                        _logger.LogWarning($"Ошибка при попытке удалить подключение у группы. Не указаны идентификаторы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке попытке удалить подключение у группы. Нет идентификаторов.",
                                buttons = new Dictionary<string, string>()
                                {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    AppUserGroup? group = await _context.UserGroups
                        .Include(usr => usr.Connections)
                        .Where(usr => usr.Id == groupId)
                        .FirstOrDefaultAsync();

                    if (group is null)
                    {
                        _logger.LogWarning($"Ошибка при попытке удалить подключение у группы. Не найдена группа.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке попытке удалить подключение у группы. Не найдена группа.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                    await DropConnection(connectionId.Value, group);
                    return View("ShowConnections", new ShowConnectionsModel(group, accessSettings));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при попытке удалить подключение у группы. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке попытке удалить подключение у группы.",
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
        /// Запрос на добавление пользователя в группу
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="groupId">Идентификатор группы</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddUser(long? userId, long? groupId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.GroupUsers != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке добавления пользователя в группу. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUserGroups\showUsers");
                    }
                    if (!userId.HasValue || !groupId.HasValue)
                    {
                        _logger.LogError($"Ошибка при попытке добавить пользователя в группу. Нет идентификаторов в запросе.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке добавить пользователя в группу. Нет идентификаторов.",
                                buttons = new Dictionary<string, string>()
                                {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    AppUserGroup? group = await _context.UserGroups
                        .Include(usr => usr.Users)
                        .Where(usr => usr.Id == groupId)
                        .FirstOrDefaultAsync();
                    if (group is null)
                    {
                        _logger.LogWarning($"Ошибка при попытке добавить пользователя в группу. Нет группы в БД.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке добавить пользователя в группу. Нет группы в БД.",
                                buttons = new Dictionary<string, string>()
                                {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                    AppUser? user = await _context.Users.FindAsync(userId);
                    if (user is null)
                    {
                        _logger.LogWarning($"Ошибка при попытке добавить пользователя в группу. Нет пользователя в БД.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке добавить пользователя в группу. Нет пользователя в БД.",
                                buttons = new Dictionary<string, string>()
                                {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                    group.Users.Add(user);
                    _context.Update(group);
                    await _context.SaveChangesAsync();
                    List<AppUser> users = _context.Users
                        .Include(usr => usr.Credentials)
                        .ToList();
                    _logger.LogInformation($"Запрос на добавление пользователя {user.Name} в группу {group.GroupName}.");
                    return View("AddUser", new AddUserModel(users, group));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при попытке добавить пользователя в группу.{ex.Message}.{ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке добавить пользователя в группу.",
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
        /// Запрос на удаление пользователя из группы
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="groupId">Идентификатор группы</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DropUserOnAddUserList(long? userId, long? groupId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.GroupUsers != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке удаления пользователя из группы. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUserGroups\showUsers");
                    }
                    if (!userId.HasValue || !groupId.HasValue)
                    {
                        _logger.LogWarning($"Ошибка при попытке удалить пользователя из группы. Нет идентификаторов.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке удалить пользователя из группы. Нет идентификаторов.",
                                buttons = new Dictionary<string, string>()
                                {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }

                    AppUserGroup? group = await _context.UserGroups
                        .Include(usr => usr.Users)
                        .Where(usr => usr.Id == groupId)
                        .FirstOrDefaultAsync();

                    if (group is null)
                    {
                        _logger.LogWarning($"Ошибка при попытке удалить пользователя из группы. Не найдена группа в БД.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке удалить пользователя из группы. Не найдена группа в БД.",
                                buttons = new Dictionary<string, string>()
                                {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }

                    await DropUser(userId.Value, group);
                    List<AppUser> users = _context.Users
                        .Include(srv => srv.Credentials)
                        .ToList();
                    _logger.LogInformation($"Запрос на удаление пользователя из группы {group.GroupName}.");
                    return View("AddUser", new AddUserModel(users, group));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при попытке удалить пользователя из группы. {ex.Message}.{ex.StackTrace}.");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке удалить пользователя из группы.",
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
        /// Запрос на удаление пользователя из группы
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="groupId">Идентификатор группы</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DropUserOnUserList(long? userId, long? groupId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.GroupUsers != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке удаления пользователя из группы. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUserGroups\showUsers");
                    }
                    if (!userId.HasValue || !groupId.HasValue)
                    {
                        _logger.LogError($"Ошибка при попытке удалить пользователя из группы. Нет идентификаторов.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке удалить пользователя из группы. Нет идентификаторов.",
                                buttons = new Dictionary<string, string>()
                                {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    AppUserGroup? group = await _context.UserGroups
                        .Include(usr => usr.Users)
                        .Where(usr => usr.Id == groupId)
                        .FirstOrDefaultAsync();

                    if (group is null)
                    {
                        _logger.LogError($"Ошибка при попытке удалить пользователя из группы. Группа не найдена в БД.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке удалить пользователя из группы. Группа не найдена в БД.",
                                buttons = new Dictionary<string, string>()
                                {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                    await DropUser(userId.Value, group);
                    _logger.LogInformation($"Запрос на удаление пользователя из группы {group.GroupName}.");
                    return View("ShowUsers", new ShowUsersModel(group, accessSettings));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при попытке удалить пользователя из группы. {ex.Message}.{ex.StackTrace}.");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке удалить пользователя из группы.",
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
        /// Общий метод удаления подключения из группы
        /// </summary>
        /// <param name="connectionId">Идентификатор подключения</param>
        /// <param name="group">Группа, в которой удаляем</param>
        /// <returns></returns>
        private async Task DropConnection(long connectionId, AppUserGroup group)
        {
            Connection? connection = await _context.Connections.FindAsync(connectionId);
            if (connection is null)
            {
                _logger.LogWarning("Ошибка при попытке удаления подключения из группы - подключение не найдено в БД.");
                return;
            }
            group.Connections.Remove(connection);
            _context.Update(group);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Общий метод удаления пользователя из группы
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="group">Группа, в которой удаляем</param>
        /// <returns></returns>
        private async Task DropUser(long userId, AppUserGroup group)
        {
            AppUser? user = await _context.Users.FindAsync(userId);
            if (user is null)
            {
                _logger.LogWarning("Ошибка при попытке удаления пользователя из группы - подключение не найдено в БД.");
                return;
            }
            group.Users.Remove(user);
            _context.Update(group);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
