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
using ConnectorCenter.Models.Repository;

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
        /// Current logger
        /// </summary>
        private readonly ILogger _logger;
        private readonly ConnectionRepository _repository;
        private readonly ServerRepository _serverRepository;
        #endregion
        #region Constructors
        public ConnectionsController(ILogger<AppUserGroupsController> logger,
            ConnectionRepository connectionRepository,
            ServerRepository serverRepository)
        {
            _repository = connectionRepository;
            _serverRepository = serverRepository;
            _logger = logger;
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
                    if (AuthorizeService.GetAccessSettings(HttpContext).ServersConnections != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу добавления подключения к серверу. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\servers");
                    }
                    Server? server = await _serverRepository.GetByIdSimple(serverId);
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
                    if (AuthorizeService.GetAccessSettings(HttpContext).ServersConnections != AccessSettings.AccessModes.Edit)
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

                    Connection? connection = await _repository.GetById(connectionId.Value);
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
                    if (AuthorizeService.GetAccessSettings(HttpContext).ServersConnections != AccessSettings.AccessModes.Edit)
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

                    Connection? connection = await _repository.GetByIdWithServerOnly(id.Value);
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
                        await _repository.Remove(connection);
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
        public async Task<IActionResult> Add(long? serverId, Connection? connection)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                if (ModelState.IsValid)
                {
                    try
                    {
                        if (AuthorizeService.GetAccessSettings(HttpContext).ServersConnections != AccessSettings.AccessModes.Edit)
                        {
                            _logger.LogWarning("Отказано в попытке добавления подключения к серверу. Недостаточно прав.");
                            return AuthorizeService.ForbiddenActionResult(this, @"\servers");
                        }

                        if(serverId is null || connection is null)
                        {
                            _logger.LogWarning($"Ошибка при запросе добавления подключения к серверу. Ошибка в агументах.");
                            return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                new
                                {
                                    message = "Ошибка при запросе добавления подключения к серверу. Ошибка в агументах.",
                                    buttons = new Dictionary<string, string>()
                                    {
                                        {"На главную",@"\dashboard" },
                                        {"К логам",@"\logs" }
                                    },
                                    errorCode = 404
                                }));
                        }

                        Server? server = await _serverRepository.GetById(serverId.Value);
                        if (server is not null)
                        {
                            connection.Server = server;
                            await _serverRepository.AddConnection(server, connection);
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
                    if (AuthorizeService.GetAccessSettings(HttpContext).ServersConnections != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке изменения подключения к серверу. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\servers");
                    }
                    if (!await _repository.ConnectionExists(connection.Id))
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
                        await _repository.Update(connection);
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
                    if (AuthorizeService.GetAccessSettings(HttpContext).ServersConnections != AccessSettings.AccessModes.Edit)
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
                    Connection? connection = await _repository.GetByIdWithServerOnly(id.Value);
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
                    await _repository.Update(connection);
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
    }
}
