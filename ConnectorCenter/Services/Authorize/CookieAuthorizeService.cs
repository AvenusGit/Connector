using ConnectorCore.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ConnectorCenter.Services.Authorize
{
    public class CookieAuthorizeService : AuthorizeService
    {
        public async static void SignIn(HttpContext context, Сredentials сredentials)
        {
            List<Claim> claims = new List<Claim> { new Claim(ClaimTypes.Name, сredentials.Login) };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignInAsync(context, new ClaimsPrincipal(claimsIdentity));
        }
        public static void SignOut(HttpContext context)
        {
            Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignOutAsync(context);
        }
    }
}
