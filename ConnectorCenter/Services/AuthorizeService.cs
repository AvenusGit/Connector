using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ConnectorCenter.Data;
using ConnectorCore.Models;
using System.Net;

namespace ConnectorCenter.Services
{
    public static class AuthorizeService
    {
        public static bool IsAuthorized(DataBaseContext dbContext, Сredentials credentials, out AppUser? user)
        {
                List<AppUser> userList =
                    dbContext.AppUsers.Where(user =>
                            user.Credentials.Login == credentials.Login
                         && user.Credentials.Password == credentials.Password).ToList();
                if (userList.Any())
                {
                    user = userList.First();
                    return true;
                }
                else
                {
                    user = null;
                    return false;
                }                  
        }

        public static JwtSecurityToken GetJwtToken(AppUser user)
        {
            IEnumerable<Claim> claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Credentials?.Login ?? "") };
            JwtSecurityToken jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)), // время действия 2 минуты
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return jwt;
        }
    }
}
