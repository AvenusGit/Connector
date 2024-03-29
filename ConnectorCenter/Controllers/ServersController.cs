﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConnectorCenter.Data;
using ConnectorCenter.Views.Servers;
using ConnectorCore.Models.Connections;
using ConnectorCore.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using ConnectorCenter.Services.Authorize;
using ConnectorCenter.Models.Settings;
using ConnectorCenter.Models.Repository;

namespace ConnectorCenter.Controllers
{
    /// <summary>
    /// Контроллер для работы с серверами
    /// </summary>
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class ServersController : Controller
    {
        #region Fields
        private readonly ServerRepository _repository;
        private readonly ILogger _logger;
        #endregion
        #region Constructors
        public ServersController(ILogger<AppUserGroupsController> logger,
            ServerRepository repository)
        {
            _repository = repository;
            _logger = logger;
        }
        #endregion
        #region GET
        /// <summary>
        /// Запрос страницы списка серверов
        /// </summary>
        /// <returns>Страница списков серверов</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                ConnectorCenterApp.Instance.Statistics.IncWebRequest();
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.Servers == AccessSettings.AccessModes.None)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу со списком серверов. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\dashboard");
                    }
                    _logger.LogInformation($"Запрос страницы списка серверов.");
                    return View(new IndexModel(await _repository.GetAll(),accessSettings));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы списка серверов. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы списка серверов.",
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
        /// Запрос страницы добавления нового сервера
        /// </summary>
        /// <returns>Страница добавления нового сервера</returns>
        [HttpGet]
        public IActionResult Add()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.Servers != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу добавления сервера. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\servers");
                    }
                    _logger.LogInformation($"Запрос страницы добавления нового сервера.");
                    return View();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы добавления нового сервера. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы добавления нового сервера.",
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
        /// Запрос страницы редактирования сервера
        /// </summary>
        /// <param name="id">Идентификатор сервера</param>
        /// <returns>Страница редактирования с заполненными полями</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(long? id)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.Servers != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу изменения сервера. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\servers");
                    }
                    if (id == null)
                    {
                        _logger.LogWarning($"Ошибка при запросе страницы редактирования сервера. Нет идентификатора в запросе.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе страницы редактирования сервера. Нет идентификатора в запросе.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    Server? server = await _repository.GetById(id.Value);
                    if (server == null)
                    {
                        _logger.LogWarning($"Ошибка при запросе страницы редактирования сервера. Сервер не найден.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе страницы редактирования сервера. Сервер не найден.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                    _logger.LogInformation($"Запрос страницы редактирования сервера {server.Name}.");
                    return View(new EditModel(server));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе страницы редактирования сервера. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы редактирования сервера.",
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
        /// Запрос на страницу со списком подключений сервера
        /// </summary>
        /// <param name="id">Идентификатор сервера</param>
        /// <returns>Страница со списком подключений сервера</returns>
        [HttpGet]
        public async Task<IActionResult> ShowConnections(long id)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    AccessSettings accessSettings = AuthorizeService.GetAccessSettings(HttpContext);
                    if (accessSettings.ServersConnections == AccessSettings.AccessModes.None)
                    {
                        _logger.LogWarning("Отказано в попытке запросить страницу подключений сервера. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\servers");
                    }
                    Server? server = await _repository.GetById(id);
                    if (server != null)
                    {
                        _logger.LogInformation($"Запрошена страница подключений сервера {server.Name}.");
                        return View(new ShowConnectionsModel(server,accessSettings));
                    }
                    else
                    {
                        _logger.LogWarning($"Ошибка при запросе страницы подключений сервера. Сервер не найден.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе страницы подключений сервера. Сервер не найден.",
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
                    _logger.LogError($"Ошибка при запросе страницы подключений сервера. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе страницы подключений сервера.",
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
        /// Запрос на добавление нового сервера
        /// </summary>
        /// <param name="server">Новый сервер</param>
        /// <returns>Страница списка серверов</returns>
        [HttpPost]
        public async Task<IActionResult> Add(Server server)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    if (AuthorizeService.GetAccessSettings(HttpContext).Servers != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке добавить сервер. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\servers");
                    }
                    if (ModelState.IsValid)
                    {
                        await _repository.Add(server);
                        _logger.LogInformation($"Добавлен новый сервер {server.Name}.");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        _logger.LogWarning($"Ошибка при попытке добавить новый сервер. Неверные аргументы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при попытке добавить новый сервер. Неверные аргументы.",
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
                    _logger.LogError($"Ошибка при попытке добавить новый сервер. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при попытке добавить новый сервер.",
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
        /// Запрос на изменение сервера
        /// </summary>
        /// <param name="id">Идентификатор сервера</param>
        /// <param name="server">Измененный сервер</param>
        /// <returns>Страница списка серверов</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(long id, Server server)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    if (AuthorizeService.GetAccessSettings(HttpContext).Servers != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке изменить сервер. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\servers");
                    }
                    if (ModelState.IsValid)
                    {
                        if (!await _repository.ServerExist(server.Id))
                        {
                            _logger.LogWarning($"Ошибка при запросе изменения сервера. Сервер не найден.");
                            return RedirectToAction("Index", "Message", new RouteValueDictionary(
                                new
                                {
                                    message = "Ошибка при запросе изменения сервера. Сервер не найден.",
                                    buttons = new Dictionary<string, string>()
                                    {
                                        {"На главную",@"\dashboard" },
                                        {"К логам",@"\logs" }
                                    },
                                    errorCode = 404
                                }));
                        }
                        await _repository.Update(server);
                        _logger.LogInformation($"Изменен сервер {server.Name}.");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        _logger.LogWarning($"Ошибка при запросе изменения сервера. Неверные аргументы.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе изменения сервера. Неверные аргументы.",
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
                    _logger.LogError($"Ошибка при запросе изменения сервера. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе изменения сервера.",
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
        /// Запрос на изменение статуса активности сервера
        /// </summary>
        /// <param name="id">Идентификатор сервера</param>
        /// <returns>Страница списка серверов</returns>
        [HttpPost]
        public async Task<IActionResult> ChangeAcessMode(long? id)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    if (AuthorizeService.GetAccessSettings(HttpContext).Servers != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке изменить статус активности сервера. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\servers");
                    }
                    if (id == null)
                    {
                        _logger.LogWarning($"Ошибка при запросе изменения статуса активности сервера. Нет идентификатора в запросе.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе изменения статуса активности сервера. Нет идентификатора в запросе.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 400
                            }));
                    }
                    Server? server = await _repository.GetByIdSimple(id.Value);
                    if (server == null)
                    {
                        _logger.LogWarning($"Ошибка при запросе изменения статуса активности сервера. Сервер не найден.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе изменения статуса активности сервера. Сервер не найден.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                    server.IsAvailable = !server.IsAvailable;
                    await _repository.Update(server);
                    _logger.LogInformation($"Изменен статус активности сервера {server.Name} с {!server.IsAvailable} на {server.IsAvailable}.");
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при запросе изменения статуса активности сервера. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе изменения статуса активности сервера.",
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
        /// Запрос на удаление сервера
        /// </summary>
        /// <param name="id">Идентификатор сервера</param>
        /// <returns>Страница списка серверов</returns>
        [HttpPost]
        public async Task<IActionResult> Delete(long? id)
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    if (AuthorizeService.GetAccessSettings(HttpContext).Servers != AccessSettings.AccessModes.Edit)
                    {
                        _logger.LogWarning("Отказано в попытке удалить сервер. Недостаточно прав.");
                        return AuthorizeService.ForbiddenActionResult(this, @"\servers");
                    }
                    if(id == null)
                    {
                        _logger.LogWarning($"Ошибка при запросе на удаление сервера. Не указан идентификатор.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе на удаление сервера. Не указан идентификатор.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"К логам",@"\logs" }
                                },
                                errorCode = 404
                            }));
                    }
                    Server? server = await _repository.GetByIdSimple(id.Value);
                    if (server != null)
                    {
                        await _repository.Remove(server);
                        _logger.LogInformation($"Удален сервер {server.Name}");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        _logger.LogWarning($"Ошибка при запросе на удаление сервера. Сервер не найден.");
                        return RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = "Ошибка при запросе на удаление сервера. Сервер не найден.",
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
                    _logger.LogError($"Ошибка при запросе на удаление сервера. {ex.Message}. {ex.StackTrace}");
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Ошибка при запросе на удаление сервера.",
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
