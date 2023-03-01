using ConnectorCenter.Data;
using ConnectorCenter.Models.Repository;
using ConnectorCenter.Models.Settings;
using ConnectorCenter.Services.Authorize;
using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using ConnectorCore.Models.VisualModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ConnectorCenter.Controllers.Api
{
    /// <summary>
    /// Api for user information request
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class AppUsersApi : ControllerBase
    {
        #region Fields
        private readonly AppUserRepository _repository;
        private readonly UserSettingsRepository _userSettingsRepository;
        private readonly ILogger _logger;
        #endregion
        #region Constructors
        public AppUsersApi(ILogger<AppUsersApi> logger,
            AppUserRepository repository,
            UserSettingsRepository userSettingsRepository)
        {
            _repository = repository;
            _logger = logger;
            _userSettingsRepository = userSettingsRepository;
        }
        #endregion
        #region Requests
        /// <summary>
        /// Full user information API request
        /// </summary>
        /// <returns>JSON full user info</returns>
        [Route("api/appuser/full")]
        [HttpGet]
        public async Task GetAppUserFull()
        {
            using (var scope = _logger.BeginScope($"API({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    if (!ConnectorCenterApp.Instance.ApiSettings.ApiEnabled)
                    {
                        _logger.LogWarning($"Отказано в запросе данных пользователя. API глобально выключен.");
                        HttpContext.Response.StatusCode = 403;
                        await HttpContext.Response.WriteAsync("API disabled in app settings.");
                        return;
                    }
                    AppUser? user = await _repository.GetByIdFull(JwtAuthorizeService.GetJwtUserId(HttpContext));
                    if (user is not null)
                    {
                        user.Credentials.Password = null; //  clear user password
                        HttpContext.Response.ContentType = "application/json";
                        await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(user, new JsonSerializerSettings
                        {
                            PreserveReferencesHandling = PreserveReferencesHandling.Objects
                        }));
                        _logger.LogInformation($"Выполнен API запрос данных пользователя. Пользователь {user.Name}.");
                    }
                    else
                    {
                        _logger.LogWarning($"Отказано в запросе данных пользователя. Пользователь не найден в БД.");
                        HttpContext.Response.StatusCode = 404;
                        await HttpContext.Response.WriteAsync("User of this token not found in database.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Ошибка при запросе данных пользователя. {ex.Message} {ex.StackTrace}");
                    HttpContext.Response.StatusCode = 500;
                    await HttpContext.Response.WriteAsync("500: Internal error");
                }
            }
        }

        /// <summary>
        /// API Request user groups
        /// </summary>
        /// <returns>JSON user groups collection</returns>
        [Route("api/appuser/groups")]
        [HttpGet]
        public async Task GetAppUserGroups()
        {
            using (var scope = _logger.BeginScope($"API({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    if (!ConnectorCenterApp.Instance.ApiSettings.ApiEnabled)
                    {
                        _logger.LogWarning($"Отказано в запросе групп пользователя. API глобально выключен.");
                        HttpContext.Response.StatusCode = 403;
                        await HttpContext.Response.WriteAsync("API disabled in app settings.");
                        return;
                    }

                    AppUser? user = await _repository.GetByIdWithGroups(JwtAuthorizeService.GetJwtUserId(HttpContext));

                    if (user is not null)
                    {
                        HttpContext.Response.ContentType = "application/json";
                        await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(user.Groups, new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }));
                        _logger.LogWarning($"Выполнен API запрос групп пользователя. Пользователь {user.Name}.");
                    }
                    else
                    {
                        _logger.LogWarning($"Отказано в запросе групп пользователя. Пользователь не найден в БД.");
                        HttpContext.Response.StatusCode = 404;
                        await HttpContext.Response.WriteAsync("User of this token not found in database.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Ошибка при запросе групп пользователя. {ex.Message} {ex.StackTrace}");
                    HttpContext.Response.StatusCode = 500;
                    await HttpContext.Response.WriteAsync("500: Internal error");
                }
            }
        }

        /// <summary>
        /// API Request user connections
        /// </summary>
        /// <returns>JSON connections collection (self and groups)</returns>
        [Route("api/appuser/connections")]
        [HttpGet]
        public async Task GetAppUserConnections()
        {
            using (var scope = _logger.BeginScope($"API({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    if (!ConnectorCenterApp.Instance.ApiSettings.ApiEnabled)
                    {
                        _logger.LogWarning($"Отказано в запросе подключений пользователя. API глобально выключен.");
                        HttpContext.Response.StatusCode = 403;
                        await HttpContext.Response.WriteAsync("API disabled in app settings.");
                        return;
                    }

                    AppUser? user = await _repository.GetByIdWithConnections(JwtAuthorizeService.GetJwtUserId(HttpContext));

                    if (user is not null)
                    {
                        List<Connection> connections = new List<Connection>();
                        connections.AddRange(user.Connections);
                        foreach (AppUserGroup group in user.Groups)
                        {
                            connections.AddRange(group.Connections);
                        }

                        HttpContext.Response.ContentType = "application/json";
                        await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(connections, new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }));
                        _logger.LogWarning($"Выполнен API запрос подключений пользователя. Пользователь {user.Name}.");
                    }
                    else
                    {
                        _logger.LogWarning($"Отказано в запросе подключений пользователя. Пользователь не найден в БД.");
                        HttpContext.Response.StatusCode = 404;
                        await HttpContext.Response.WriteAsync("User of this token not found in database.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Ошибка при запросе подключений пользователя. {ex.Message} {ex.StackTrace}");
                    HttpContext.Response.StatusCode = 500;
                    await HttpContext.Response.WriteAsync("500: Internal error");
                }
            }
        }

        /// <summary>
        /// API Request user settings
        /// </summary>
        /// <returns>JSON user settings</returns>
        [Route("api/appuser/userSettings")]
        [HttpGet]
        public async Task GetAppUserSettings()
        {
            using (var scope = _logger.BeginScope($"API({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    if (!ConnectorCenterApp.Instance.ApiSettings.ApiEnabled)
                    {
                        _logger.LogWarning($"Отказано в запросе пользовательских настроек. API глобально выключен.");
                        HttpContext.Response.StatusCode = 403;
                        await HttpContext.Response.WriteAsync("API disabled in app settings.");
                        return;
                    }

                    UserSettings? userSettings = await _userSettingsRepository.GetByUserId(JwtAuthorizeService.GetJwtUserId(HttpContext));

                    if (userSettings is not null)
                    {
                        HttpContext.Response.ContentType = "application/json";
                        await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(userSettings, new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }));
                        _logger.LogWarning($"Выполнен API запрос пользовательских настроек.");
                    }
                    else
                    {
                        _logger.LogWarning($"Отказано в запросе пользовательских настроек. Настройки не найдены в БД.");
                        HttpContext.Response.StatusCode = 404;
                        await HttpContext.Response.WriteAsync("Settings of this user not found in database.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Ошибка при запросе пользовательских настроек. {ex.Message} {ex.StackTrace}");
                    HttpContext.Response.StatusCode = 500;
                    await HttpContext.Response.WriteAsync("500: Internal error");
                }
            }
        }

        /// <summary>
        /// API Request user visual scheme
        /// </summary>
        /// <returns>JSON user, only with user visual scheme information</returns>
        [Route("api/appuser/visualScheme")]
        [HttpGet]
        public async Task GetAppUserVisualScheme()
        {
            using (var scope = _logger.BeginScope($"API({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    if (!ConnectorCenterApp.Instance.ApiSettings.ApiEnabled)
                    {
                        _logger.LogWarning($"Отказано в запросе визуальных настроек. API глобально выключен.");
                        HttpContext.Response.StatusCode = 403;
                        await HttpContext.Response.WriteAsync("API disabled in app settings.");
                        return;
                    }

                    VisualScheme? visualScheme = await _repository.GetVisualScheme(JwtAuthorizeService.GetJwtUserId(HttpContext));

                    if (visualScheme is not null)
                    {
                        HttpContext.Response.ContentType = "application/json";
                        await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(visualScheme, new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }));
                        _logger.LogWarning($"Выполнен API запрос визуальных настроек.");
                    }
                    else
                    {
                        _logger.LogWarning($"Отказано в запросе визуальных настроек. Настройки не найдены в БД.");
                        HttpContext.Response.StatusCode = 404;
                        await HttpContext.Response.WriteAsync("Settings of this user not found in database.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Ошибка при запросе визуальных настроек. {ex.Message} {ex.StackTrace}");
                    HttpContext.Response.StatusCode = 500;
                    await HttpContext.Response.WriteAsync("500: Internal error");
                }
            }
        }

        /// <summary>
        /// API Request user access settings
        /// </summary>
        /// <returns>JSON user access settings</returns>
        [Route("api/appuser/access")]
        [HttpGet]
        public async Task GetAppUserAccess()
        {
            using (var scope = _logger.BeginScope($"API({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    if (!ConnectorCenterApp.Instance.ApiSettings.ApiEnabled)
                    {
                        _logger.LogWarning($"Отказано в запросе доступов. API глобально выключен.");
                        HttpContext.Response.StatusCode = 403;
                        await HttpContext.Response.WriteAsync("API disabled in app settings.");
                        return;
                    }

                    AccessSettings accessSettings = JwtAuthorizeService.GetAccessSettings(JwtAuthorizeService.GetUserRole(HttpContext));
                    HttpContext.Response.ContentType = "application/json";
                    await HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(accessSettings, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }));
                    _logger.LogWarning($"Выполнен API запрос доступов.");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Ошибка при запросе доступов. {ex.Message} {ex.StackTrace}");
                    HttpContext.Response.StatusCode = 500;
                    await HttpContext.Response.WriteAsync("500: Internal error");
                }
            }
        }

        /// <summary>
        /// API Request on change user visual scheme
        /// </summary>
        /// <returns>StatusCode</returns>
        [Route("api/appuser/visualScheme/set")]
        [HttpPost]
        public async Task SetAppUserVisualScheme([FromBody] VisualScheme? scheme)
        {
            using (var scope = _logger.BeginScope($"API({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    if (scheme is not null)
                    {
                        AppUser? user = await _repository.GetByIdAllSettings(JwtAuthorizeService.GetJwtUserId(HttpContext));
                        if (user is not null)
                        {
                            user.VisualScheme = scheme;
                            await _repository.Update(user);
                            _logger.LogInformation($"Изменена визуальная схема пользователя {user.Name} через API.");
                            HttpContext.Response.StatusCode = 200;
                            await HttpContext.Response.WriteAsync("200: Visual scheme succefull changed.");
                        }
                        else
                        {
                            _logger.LogWarning($"Ошибка при попытке установки визуальных настроек. Пользователь не найден в БД.");
                            HttpContext.Response.StatusCode = 404;
                            await HttpContext.Response.WriteAsync("404: User not found");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Ошибка при попытке установки визуальных настроек. Отсутствуют аргументы.");
                        HttpContext.Response.StatusCode = 400;
                        await HttpContext.Response.WriteAsync("400: Argument error");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Ошибка при попытке установки визуальных настроек. {ex.Message} {ex.StackTrace}");
                    HttpContext.Response.StatusCode = 500;
                    await HttpContext.Response.WriteAsync("500: Internal error");
                }
            }
        }

        /// <summary>
        /// API Request on change user rdp settings
        /// </summary>
        /// <returns>StatusCode</returns>
        [Route("api/appuser/rdpsettings/set")]
        [HttpPost]
        public async Task SetAppUserRdpSettings([FromBody] RdpSettings? rdpSettings)
        {
            using (var scope = _logger.BeginScope($"API({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                    if (rdpSettings is not null)
                    {
                        AppUser? user = await _repository.GetById(JwtAuthorizeService.GetJwtUserId(HttpContext));
                        if (user is not null)
                        {
                            if (user.UserSettings is null)
                                user.UserSettings = UserSettings.GetDefault();
                            user.UserSettings.RdpSettings = rdpSettings;
                            await _repository.Update(user);
                            _logger.LogInformation($"Изменены настройки RDP пользователя {user.Name} через API.");
                            HttpContext.Response.StatusCode = 200;
                            await HttpContext.Response.WriteAsync("200: Rdp settings succefull changed.");
                        }
                        else
                        {
                            _logger.LogWarning($"Ошибка при попытке установки визуальных настроек. Пользователь не найден в БД.");
                            HttpContext.Response.StatusCode = 404;
                            await HttpContext.Response.WriteAsync("404: User not found");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Ошибка при попытке установки визуальных настроек. Отсутствуют аргументы.");
                        HttpContext.Response.StatusCode = 400;
                        await HttpContext.Response.WriteAsync("400: Argument error");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Ошибка при попытке установки визуальных настроек. {ex.Message} {ex.StackTrace}");
                    HttpContext.Response.StatusCode = 500;
                    await HttpContext.Response.WriteAsync("500: Internal error");
                }
            }
        }
        #endregion
    }
}
