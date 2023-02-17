using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConnectorCenter.Views.Connections;
using Microsoft.EntityFrameworkCore;
using ConnectorCenter.Data;
using ConnectorCore.Models.Connections;
using ConnectorCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using log4net.Repository.Hierarchy;
using ConnectorCenter.Services.Authorize;
using ConnectorCenter.Models.Settings;

namespace ConnectorCenter.Controllers
{
    /// <summary>
    /// Connections controller
    /// </summary>
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class ConnectionsController : Controller
    {
        #region Fields
        /// <summary>
        /// Current access setting for current user
        /// </summary>
        private readonly AccessSettings _accessSettings;
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
        public ConnectionsController(DataBaseContext context, ILogger<AppUserGroupsController> logger)
        {
            _context = context;
            _logger = logger;
            _accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
        }
        #endregion
        #region GET

        /// <summary>
        /// GET: add new connection request
        /// </summary>
        /// <param name="serverId">Server identifier</param>
        /// <returns>Add new connection page</returns>
        [HttpGet]
        public async Task<IActionResult> Add(long serverId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (_accessSettings.ServersConnections != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу добавления подключения к серверу. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\servers");
                    }
                    Server? server = await _context.Servers.FindAsync(serverId);
                    if (server is not null)
                    {
                        _logger.LogInformation($"Запрос страницы добавления нового подключения для сервера {server.Name}.S");
                        return View(new AddModel(server));
                    }
                    else
                    {
                        _logger.LogWarning($"Ошибка при запросе страницы добавления подключения. Указанный сервер не существует.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе страницы добавления подключения. Указанный сервер не существует.",
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
                    _logger.LogError($"Ошибка при запросе страницы добавления подключения. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы добавления подключения.",
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
        /// GET: edit connection page request
        /// </summary>
        /// <param name="connectionId">Connection identifier</param>
        /// <returns>Edit connection page</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(long? connectionId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (_accessSettings.ServersConnections != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу изменения подключения к серверу. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\servers");
                    }
                    if (connectionId == null)
                    {
                        _logger.LogWarning($"Ошибка при запросе страницы редактирования подключения. Не указан идентификатор подключения.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе страницы редактирования подключения. Не указан идентификатор подключения.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }

                    Connection? connection = await _context.Connections
                        .Include(conn => conn.Server)
                        .Include(conn => conn.ServerUser)
                            .ThenInclude(user => user!.Credentials)
                        .Where(conn => conn.Id == connectionId)
                        .FirstOrDefaultAsync();
                    if (connection == null)
                    {
                        _logger.LogWarning($"Ошибка при запросе страницы редактирования подключения. Подключение на найдено.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе страницы редактирования подключения. Подключение на найдено.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                    _logger.LogInformation($"Запрос страницы редактирования подключения {connection.ConnectionName} у сервера {connection.Server.Name}");
                    return View(new EditModel(connection));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы редактирования подключения. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы редактирования подключения.",
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
        //TODO change to POST or DELETE type!
        /// <summary>
        /// GET: Delete connection request 
        /// </summary>
        /// <param name="id">Connection identifier</param>
        /// <returns>Connection list page</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(long? id)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (_accessSettings.ServersConnections != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке удаления подключения к серверу. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\servers");
                    }
                    if (id == null)
                    {
                        _logger.LogWarning($"Ошибка при запросе удаления подключения. Не указан идентификатор.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе удаления подключения. Не указан идентификатор.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }

                    Connection? connection = await _context.Connections
                        .Include(srv => srv.Server)
                        .FirstOrDefaultAsync(m => m.Id == id);
                    if (connection == null)
                    {
                        _logger.LogWarning($"Ошибка при запросе удаления подключения. Подключение не найдено.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе удаления подключения. Подключение не найдено.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                    else
                    {
                        _context.Connections.Remove(connection);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"Удалено подключение {connection.ConnectionName} у сервера {connection.Server.Name}.");
                        return RedirectToAction("ShowConnections", "Servers", new RouteValueDictionary(
                                        new { id = connection.Server.Id }));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе удаления подключения. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе удаления подключения.",
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
        /// POST: request on add new connection to server
        /// </summary>
        /// <param name="serverId">Server identifier</param>
        /// <param name="connection">New connection instance</param>
        /// <returns>Server's connection list page</returns>
        [HttpPost]
        public async Task<IActionResult> Add(long serverId, Connection connection)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                if (ModelState.IsValid)
                {
                    try
                    {
                        if (_accessSettings.ServersConnections != AccessSettings.AccessModes.Edit)
                        {
                            _logger.LogWarning("Отказано в попытке добавления подключения к серверу. Недостаточно прав.");
                            return AuthorizeService.ForbiddenActionResult(this, @"\servers");
                        }
                        Server? server = await _context.Servers.FindAsync(serverId);
                        if (server is not null)
                        {
                            connection.Server = server;
                            server.Connections.Add(connection);
                            _context.Update(server);
                            await _context.SaveChangesAsync();
                            _logger.LogInformation($"Серверу {server.Name} добавлено новое подключение {connection.ConnectionName}.");
                            return RedirectToAction("ShowConnections", "Servers", new RouteValueDictionary(
                                        new { id = connection.Server.Id }));
                        }
                        else
                        {
                            _logger.LogWarning($"Ошибка при запросе добавления подключения к серверу. Сервер не найден.");
                            return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                new
                                {
                                    message = "Ошибка при запросе добавления подключения к серверу. Сервер не найден.",
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
                        _logger.LogError($"Ошибка при запросе добавления подключения к серверу. {ex.Message}. {ex.StackTrace}");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе добавления подключения к серверу.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 500
                            }));
                    }
                }
                else
                {
                    _logger.LogWarning($"Ошибка при запросе добавления подключения к серверу. Неверные аргументы.");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе добавления подключения к серверу. Неверные аргументы.",
                            buttons = new Dictionary<string, string>()
                            {
                                        {"На главную",@"\dashboard" },
                                        {"К логам",@"\logs" }
                            },
                            errorCode = 400
                        }));
                }
            }
        }

        /// <summary>
        /// POST: edit connection request
        /// </summary>
        /// <param name="serverId">Server identifier</param>
        /// <param name="connection">Connection edited instance</param>
        /// <returns>Server's connection list</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(long serverId, Connection connection)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (_accessSettings.ServersConnections != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке изменения подключения к серверу. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\servers");
                    }
                    if (!ConnectionExists(connection.Id))
                    {
                        _logger.LogWarning($"Ошибка при запросе изменения подключения. Указанного подключения не существует.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе изменения подключения. Подключение не найдено.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }

                    if (ModelState.IsValid)
                    {
                        _context.Update(connection);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"Изменено подключение {connection.ConnectionName}.");
                        return RedirectToAction("ShowConnections", "Servers", new RouteValueDictionary(
                                        new { id = serverId }));
                    }
                    else
                    {
                        _logger.LogWarning($"Ошибка при запросе изменения подключения. Неверные аргументы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе изменения подключения. Неверные аргументы.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе изменения подключения. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе изменения подключения.",
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
        /// POST: change access mode request
        /// </summary>
        /// <param name="id">Connection identifier</param>
        /// <returns>Server's connection list</returns>
        [HttpPost]
        public async Task<IActionResult> ChangeAcessMode(long? id)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                    if (_accessSettings.ServersConnections != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке изменения статуса активности подключения к серверу. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\servers");
                    }
                    if (id == null)
                    {
                        _logger.LogError($"Ошибка при запросе изменения статуса активности подключения. Неверные аргументы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе изменения статуса активности подключения. Неверные аргументы.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    Connection? connection = await _context.Connections
                        .Include(conn => conn.Server)
                        .Where(conn => conn.Id == id)
                        .FirstOrDefaultAsync();
                    if (connection == null)
                    {
                        _logger.LogError($"Ошибка при запросе изменения статуса активности подключения. Подключение не найдено.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе изменения статуса активности подключения. Подключение не найдено.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                    connection.IsAvailable = !connection.IsAvailable;
                    _context.Update(connection);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Изменен статус активности подключения {connection.ConnectionName} с {!connection.IsAvailable} на {connection.IsAvailable}");
                    return RedirectToAction("ShowConnections", "Servers", new RouteValueDictionary(
                                        new { id = connection.Server.Id }));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе изменения статуса активности подключения. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе изменения статуса активности подключения.",
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
        /// Check on exisisting connection in database
        /// </summary>
        /// <param name="id">Connection identifier</param>
        /// <returns>True - connection exist, otherwise - false.</returns>
        private bool ConnectionExists(long id)
        {
            return (_context.Connections?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        #endregion
    }
}
