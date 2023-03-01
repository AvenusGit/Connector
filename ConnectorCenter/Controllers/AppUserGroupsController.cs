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
using ConnectorCenter.Models.Repository;

namespace ConnectorCenter.Controllers
{
    /// <summary>
    /// Application user's groups controller
    /// </summary>
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class AppUserGroupsController : Controller
    {
        #region Fields
        /// <summary>
        /// UserGroups Repository
        /// </summary>
        private readonly AppUserGroupRepository _repository;
        private readonly ServerRepository _serverRepository;
        private readonly AppUserRepository _userRepository;
        private readonly ConnectionRepository _connectionRepository;
        /// <summary>
        /// Current logger
        /// </summary>
        private readonly ILogger _logger;
        /// <summary>
        /// Database context
        /// </summary>
        //private readonly DataBaseContext _context;
        #endregion
        #region Constructors
        public AppUserGroupsController( 
            ILogger<AppUserGroupsController> logger,
            AppUserGroupRepository appUserGrouprepository,
            ServerRepository serverRepository,
            AppUserRepository userRepository,
            ConnectionRepository connectionRepository)
        {
            _serverRepository = serverRepository;
            _repository = appUserGrouprepository;
            _userRepository = userRepository;
            _connectionRepository = connectionRepository;
            _logger = logger;
        }
        #endregion
        #region GET
        /// <summary>
        /// Get: Request appuser groups page.
        /// </summary>
        /// <returns>Page</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    AccessSettings currentAcessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (currentAcessSettings.Groups == AccessSettings.AccessModes.None)
                    {
                        _logger.LogWarning("Отказано в попытке запросить список групп. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\dashboard");
                    }
                    _logger.LogInformation("Запрошен список групп пользователей.");
                    return View(new IndexModel(
                        await _repository.GetAll(),
                        currentAcessSettings));
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
        /// Get: Request add new app user group page
        /// </summary>
        /// <returns>Page</returns>
        [HttpGet]
        public IActionResult Add()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {                    
                    if (AuthorizeService.GetAccessSettings(HttpContext).Groups != AccessSettings.AccessModes.Edit)
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
        /// Get: Request edit of app user groups
        /// </summary>
        /// <param name="id">pp user groups identifier</param>
        /// <returns>Index Page</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(long? id)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {                   
                    if (AuthorizeService.GetAccessSettings(HttpContext).Groups != AccessSettings.AccessModes.Edit)
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

                    AppUserGroup? appUserGroup = await _repository.GetByIdSimple(id.Value);
                    if (appUserGroup is null)
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
        /// Get: Request connections of app user groups
        /// </summary>
        /// <param name="groupId">App user groups identifier</param>
        /// <returns>Connections page</returns>
        [HttpGet]
        public async Task<IActionResult> ShowConnections(long? groupId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    AccessSettings currentAcessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (currentAcessSettings.GroupConnections == AccessSettings.AccessModes.None)
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

                    AppUserGroup? userGroup = await _repository.GetByIdWithConnections(groupId.Value);
                    if (userGroup != null)
                    {
                        _logger.LogInformation($"Запрошены подключения группы пользователей:{userGroup.GroupName}."); ;
                        return View(new ShowConnectionsModel(userGroup, currentAcessSettings));
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
        /// Get: Request add new connection for app user group page
        /// </summary>
        /// <param name="groupId">App user group identifier</param>
        /// <returns>New connection page</returns>
        [HttpGet]
        public async Task<IActionResult> AddConnections(long? groupId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {                    
                    if (AuthorizeService.GetAccessSettings(HttpContext).GroupConnections != AccessSettings.AccessModes.Edit)
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
                    AppUserGroup? group = await _repository.GetByIdWithConnections(groupId.Value);
                    IEnumerable<Server> servers = await _serverRepository.GetAll();
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
        /// Get: Request of user in app user groups
        /// </summary>
        /// <param name="groupId">App user group identifier</param>
        /// <returns>User list page</returns>
        [HttpGet]
        public async Task<IActionResult> ShowUsers(long? groupId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    AccessSettings currentAcessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (currentAcessSettings.GroupUsers == AccessSettings.AccessModes.None)
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

                    AppUserGroup? userGroup = await _repository.GetByIdUsersOnly(groupId.Value);
                    if (userGroup != null)
                    {
                        _logger.LogInformation($"Запрос на страницу пользователй группы {userGroup.GroupName}");
                        return View(new ShowUsersModel(userGroup, currentAcessSettings));
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
        /// Get: Request add new user in app user group
        /// </summary>
        /// <param name="groupId">App user group identifier</param>
        /// <returns>Add user page</returns>
        [HttpGet]
        public async Task<IActionResult> AddUser(long? groupId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {                    
                    if (AuthorizeService.GetAccessSettings(HttpContext).GroupUsers != AccessSettings.AccessModes.Edit)
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
                    AppUserGroup? group = await _repository.GetByIdUsersOnly(groupId.Value);
                    IEnumerable<AppUser> users = await _userRepository.GetAllCredentialsOnly();
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
        /// POST: Request to add new group
        /// </summary>
        /// <param name="appUserGroup">New group</param>
        /// <returns>Group list page</returns>
        [HttpPost]
        public async Task<IActionResult> Add(AppUserGroup appUserGroup)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();                    
                    if (AuthorizeService.GetAccessSettings(HttpContext).Groups != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке добавления группы. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUserGroups");
                    }
                    if (ModelState.IsValid)
                    {
                        await _repository.Add(appUserGroup);
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
        /// POST: Request for edit group
        /// </summary>
        /// <param name="appUserGroup">Edited group (contains Id)</param>
        /// <returns>Group list page</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(AppUserGroup appUserGroup)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {                    
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetAccessSettings(HttpContext).Groups != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке изменения группы. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUserGroups");
                    }
                    if (ModelState.IsValid)
                    {
                        await _repository.Update(appUserGroup);
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
        /// POST: Delete group request
        /// </summary>
        /// <param name="id">Group identifier</param>
        /// <returns>GroupList page</returns>
        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetAccessSettings(HttpContext).Groups != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке удаления группы. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\appUserGroups");
                    }

                    AppUserGroup? deletedGroup = await _repository.RemoveById(id);
                    if (deletedGroup is not null)
                    {
                        _logger.LogInformation($"Удалена группа пользователей:{deletedGroup.GroupName}."); ;
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
        /// POST: Add connection to group request
        /// </summary>
        /// <param name="connectionId">Connection identifier</param>
        /// <param name="groupId">Group identifier</param>
        /// <returns>Back to add connection page</returns>
        [HttpPost]
        public async Task<IActionResult> AddConnections(long? connectionId, long? groupId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetAccessSettings(HttpContext).GroupConnections != AccessSettings.AccessModes.Edit)
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

                    AppUserGroup? group = await _repository.GetByIdWithConnections(groupId.Value);
                    Connection? connection = await _connectionRepository.GetById(connectionId.Value);
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
                    if(group is null)
                    {
                        _logger.LogWarning($"Ошибка при попытке добавить подключение группе пользователей. Не найдена группа.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке добавить подключение группе пользователей. Не найдена группа.",
                                buttons = new Dictionary<string, string>()
                                {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                    await _repository.AddConnection(group, connection);
                    //group.Connections.Add(connection);
                    //_context.Update(group);
                    //await _context.SaveChangesAsync();
                    IEnumerable<Server> servers = await _serverRepository.GetAll();
                    //List<Server> servers = _context.Servers
                    //    .Include(srv => srv.Connections)
                    //        .ThenInclude(conn => conn.ServerUser)
                    //            .ThenInclude(usr => usr!.Credentials)
                    //    .ToList();
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
        /// POST Remove connection - add connection list page version
        /// </summary>
        /// <param name="connectionId">Connection identifier</param>
        /// <param name="groupId">Group identifier</param>
        /// <returns>Back to add connection page</returns>
        [HttpPost]
        public async Task<IActionResult> DropConnectionOnAddConnectionList(long? connectionId, long? groupId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetAccessSettings(HttpContext).GroupConnections != AccessSettings.AccessModes.Edit)
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

                    AppUserGroup? group = await _repository.GetByIdWithConnections(groupId.Value);
                    if (group is null)
                    {
                        _logger.LogError($"Ошибка при попытке удалить подключение у группы. Не найдена группа.");
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

                    Connection? deletedConnection = await _repository.DeleteConnection(group, connectionId.Value);
                    if(deletedConnection is null)
                        _logger.LogWarning($"Ошибка при попытке удалить подключение у группы. Не найдено указанное подключение.");

                    //await DropConnection(connectionId.Value, group);
                    IEnumerable<Server> servers = await _serverRepository.GetAll();
                    //List<Server> servers = _context.Servers
                    //    .Include(srv => srv.Connections)
                    //        .ThenInclude(conn => conn.ServerUser)
                    //            .ThenInclude(usr => usr!.Credentials)
                    //    .ToList();
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
        /// POST: Remove connection - connection list version
        /// </summary>
        /// <param name="connectionId">Connection identifier</param>
        /// <param name="groupId">Group identifier</param>
        /// <returns>Back to connection list page</returns>
        [HttpPost]
        public async Task<IActionResult> DropConnectionOnConnectionList(long? connectionId, long? groupId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    AccessSettings currentAcessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (currentAcessSettings.GroupConnections != AccessSettings.AccessModes.Edit)
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
                    AppUserGroup? group = await _repository.GetByIdWithConnections(groupId.Value);
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
                    Connection? deletedConnection = await _repository.DeleteConnection(group, connectionId.Value);
                    if (deletedConnection is null)
                        _logger.LogWarning($"Ошибка при попытке удалить подключение у группы. Не найдено указанное подключение.");
                    //await DropConnection(connectionId.Value, group);
                    return View("ShowConnections", new ShowConnectionsModel(group, currentAcessSettings));
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
        /// POST: Add user to group
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="groupId">Group identifier</param>
        /// <returns>Back to add user page</returns>
        [HttpPost]
        public async Task<IActionResult> AddUser(long? userId, long? groupId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetAccessSettings(HttpContext).GroupUsers != AccessSettings.AccessModes.Edit)
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

                    AppUserGroup? group = await _repository.GetByIdUsersOnly(groupId.Value);
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
                    AppUser? user = await _userRepository.GetById(userId.Value);
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
                    await _repository.AddUser(group, user);
                    IEnumerable<AppUser> users = await _userRepository.GetAllCredentialsOnly();
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
        /// POST Remove user from group - add user page version
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="groupId">Group identifier</param>
        /// <returns>Back to add user page</returns>
        [HttpPost]
        public async Task<IActionResult> DropUserOnAddUserList(long? userId, long? groupId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (AuthorizeService.GetAccessSettings(HttpContext).GroupUsers != AccessSettings.AccessModes.Edit)
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
                    AppUserGroup? group = await _repository.GetByIdUsersOnly(groupId.Value);
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



                    await _repository.DeleteUser(group, userId.Value);
                    IEnumerable<AppUser> users = await _userRepository.GetAllCredentialsOnly();
                    //List<AppUser> users = _context.Users
                    //    .Include(srv => srv.Credentials)
                    //    .ToList();
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
        /// POST: Remove user from group - user list page version
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="groupId">Group identifier</param>
        /// <returns>User list page</returns>
        [HttpPost]
        public async Task<IActionResult> DropUserOnUserList(long? userId, long? groupId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {

                    AccessSettings currentAcessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (currentAcessSettings.GroupUsers != AccessSettings.AccessModes.Edit)
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
                    AppUserGroup? group = await _repository.GetByIdUsersOnly(groupId.Value);
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
                    await _repository.DeleteUser(group, userId.Value);
                    //await DropUser(userId.Value, group);
                    _logger.LogInformation($"Запрос на удаление пользователя из группы {group.GroupName}.");
                    return View("ShowUsers", new ShowUsersModel(group, currentAcessSettings));
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
    }
}
