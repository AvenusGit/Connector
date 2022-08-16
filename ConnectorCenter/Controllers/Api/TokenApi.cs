using ConnectorCenter.Data;
using ConnectorCenter.Services.Authorize;
using ConnectorCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;

namespace ConnectorCenter.Controllers.Api
{
    [Route("/api/token")]
    [AllowAnonymous]
    [ApiController]
    public class TokenApi : ControllerBase
    {
        private readonly DataBaseContext _dataBaseContext;
        public TokenApi(DataBaseContext context)
        {
            _dataBaseContext = context;
        }

        [HttpGet]
        private async Task Index()
        {
            Response.StatusCode = 400;
            await HttpContext.Response.WriteAsync("JSON (Credentials{string Login, string Password}) in request body need!");
        }

        [Route("/api/token/gettoken")]
        [Produces(MediaTypeNames.Application.Json)]
        [Consumes("application/json")]
        [HttpPost]
        public async Task GetToken([FromBody] Сredentials credentials)
        {
            if(!ModelState.IsValid)
            {
                await HttpContext.Response.WriteAsync("Your model is not valid. Use {string Login, string Password}");
                return;
            }
            using (DataBaseContext db = _dataBaseContext)
            {
                AppUser? user;
                if (AuthorizeService.IsAuthorized(_dataBaseContext, credentials, out user))
                {
                    JwtSecurityToken jwt = JwtAuthorizeService.GetJwtToken(user!);
                    Response.StatusCode = 200;
                    JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                    var json = new
                    {
                        token = handler.WriteToken(jwt),
                        user = user!.Name
                    };
                    await HttpContext.Response.WriteAsJsonAsync(json);
                    return;
                }
                else
                {
                    Response.StatusCode = 401;
                    await HttpContext.Response.WriteAsync("Wrong login or password");
                    return;
                }                
            }
        }
    }
}
