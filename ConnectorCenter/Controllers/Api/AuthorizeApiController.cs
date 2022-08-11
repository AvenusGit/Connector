using ConnectorCenter.Data;
using ConnectorCenter.Services;
using ConnectorCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using System.Net.Mime;

namespace ConnectorCenter.Controllers.Api
{
    [Route("/api/authorization")]
    [ApiController]
    public class Authorization : ControllerBase
    {
        private readonly DataBaseContext _dataBaseContext;
        public Authorization(DataBaseContext context)
        {
            _dataBaseContext = context;
        }

        [HttpGet]
        private async Task Index()
        {
            await HttpContext.Response.WriteAsync("JSON (Credentials{string Login, string Password}) ib request body need!");
        }

        [Route("/api/authorization/authorize")]
        [Produces(MediaTypeNames.Application.Json)]
        [Consumes("application/json")]
        [HttpPost]
        [AllowAnonymous]
        public async Task Authorize([FromBody] Сredentials credentials)
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
                    JwtSecurityToken jwt = AuthorizeService.GetJwtToken(user!);
                    string token = new JwtSecurityTokenHandler().WriteToken(jwt);
                    var response = new
                    {
                        token = token,
                        username = user!.Name
                    };
                    await HttpContext.Response.WriteAsJsonAsync(response);
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
