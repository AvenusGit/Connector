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

namespace ConnectorCenter.Controllers
{
    /// <summary>
    /// Контроллер для работы с подключениями
    /// </summary>
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class ConnectionsController : Controller
    {
        #region Fields
        private readonly DataBaseContext _context;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        #endregion
        #region Constructors
        public ConnectionsController(DataBaseContext context, ILogger<AppUserGroupsController> logger)
        {
            _context = context;
            _logger = logger;
        }
        #endregion
        #region GET
        /// <summary>
        /// Запрос на страницу создания нового подключения
        /// </summary>
        /// <param name="serverId">Идентификатор сервера</param>
        /// <returns>Страница создания нового подключения у указанного сервера</returns>
        [HttpGet]
        public async Task<IActionResult> Add(long serverId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
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
        /// Запрос страницы редактирования подключения
        /// </summary>
        /// <param name="connectionId">Идентификатор подключения</param>
        /// <returns>Страница редатирования подключения с заполненными полями</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(long? connectionId)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
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
        /// <summary>
        /// Запрос на удаление подключения
        /// </summary>
        /// <param name="id">Идентификатор подключения</param>
        /// <returns>Страница подключений</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(long? id)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
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
        /// Запрос на добавление нового подключения к серверу
        /// </summary>
        /// <param name="serverId">Идентификатор сервера</param>
        /// <param name="connection">Новое подключение</param>
        /// <returns>Страница подключений к серверу</returns>
        [HttpPost]
        public async Task<IActionResult> Add(long serverId, Connection connection)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                if (ModelState.IsValid)
                {
                    try
                    {
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
        /// Запрос на изменение подключения
        /// </summary>
        /// <param name="serverId">Идентификатор сервера</param>
        /// <param name="connection">Измененное подключение</param>
        /// <returns>Страница подключений сервера</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(long serverId, Connection connection)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
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
        /// Запрос на изменение статуса активности подключения
        /// </summary>
        /// <param name="id">Идентификатор подключения</param>
        /// <returns>Страница подключений</returns>
        [HttpPost]
        public async Task<IActionResult> ChangeAcessMode(long? id)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
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
        /// Проверка на существование подключения в БД
        /// </summary>
        /// <param name="id">Идентификатор подключения</param>
        /// <returns>True - подключение есть, иначе - нет.</returns>
        private bool ConnectionExists(long id)
        {
            return (_context.Connections?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        #endregion
    }
}
