using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ConnectorCenter.Data;
using ConnectorCore.Models;
using System.Net;
using NuGet.Protocol.Plugins;

namespace ConnectorCenter.Services.Authorize
{
    public class JwtAuthorizeService : AuthorizeService
    {
        public static JwtSecurityToken GetJwtToken(AppUser user)
        {
            IEnumerable<Claim> claims = GetUserClaims(user);
            ClaimsIdentity identity = new ClaimsIdentity(claims, "Token");
            JwtSecurityToken jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: identity.Claims,
                expires: DateTime.UtcNow.AddMinutes(30), // время действия 30 минут
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return jwt;
        }
    }
}
