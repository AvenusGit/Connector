using ConnectorCenter.Data;
using ConnectorCenter.Services.Authorize;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConnectorCenter.Controllers.Api
{    
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class AppUsersApi : ControllerBase
    {
        #region Fields
        private readonly DataBaseContext _context;
        private readonly ILogger _logger;
        #endregion
        #region Constructors
        public AppUsersApi(DataBaseContext context, ILogger<AppUserGroupsController> logger)
        {
            _context = context;
            _logger = logger;
        }
        #endregion
        [Route("api/appuser/full")]
        public async Task<IActionResult> GetAppUserFull()
        {
            using (var scope = _logger.BeginScope($"WEB({AuthorizeService.GetUserName(HttpContext)}:{HttpContext.Connection.RemoteIpAddress}"))
            {
                try
                {
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
