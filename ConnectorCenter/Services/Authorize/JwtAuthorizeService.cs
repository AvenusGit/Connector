using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ConnectorCenter.Data;
using ConnectorCore.Models;
using System.Net;
using NuGet.Protocol.Plugins;
using ConnectorCenter.Models.Settings;
using Microsoft.IdentityModel.Logging;
using System.Text;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;

namespace ConnectorCenter.Services.Authorize
{
    public class JwtAuthorizeService : AuthorizeService
    {
        public JwtAuthorizeService(DataBaseContext dataBaseContext) : base(dataBaseContext) { }
        public static JwtSecurityToken GetJwtToken(AppUser user)
        {
            IEnumerable<Claim> claims = GetUserClaims(user);
            ClaimsIdentity identity = new ClaimsIdentity(claims, "Token");
            JwtSecurityToken jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: identity.Claims,
                expires: DateTime.UtcNow.AddMinutes(UnitedSettings.JwtTokenLifeTimeMinutes), // время действия ожидается одинаковым на сервере и клиенте
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return jwt;
        }
        public static string GetTokenFromRequest(HttpContext context)
        {
            StringValues authorization = context.Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                if(headerValue.Scheme == "Bearer" && !String.IsNullOrEmpty(headerValue.Parameter))
                    return headerValue.Parameter!;
                else throw new Exception("Тип аутентификации не Bearer.");
            }
            throw new Exception("Не удалось определить тип аутентификации");
        }
        public static ClaimsPrincipal GetClaimsFromToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();
            validationParameters.ValidateLifetime = true;
            validationParameters.ValidAudience = AuthOptions.AUDIENCE;
            validationParameters.ValidIssuer = AuthOptions.ISSUER;
            validationParameters.IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey();
            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);
            return principal;
        }
        public static long GetJwtUserId(HttpContext context)
        {
            string token = GetTokenFromRequest(context);
            ClaimsPrincipal claims = GetClaimsFromToken(token);
            long result;
            if (long.TryParse(claims.FindFirstValue(ClaimTypes.Sid), out result))
                return result;
            else throw new Exception("Не удалось определить идентификатор пользователя из JWT токена.");
        }
        public static string GetJwtUserName(HttpContext context)
        {
            string token = GetTokenFromRequest(context);
            ClaimsPrincipal claims = GetClaimsFromToken(token);
            return String.IsNullOrEmpty(claims.FindFirstValue(ClaimTypes.Sid)) 
                ? "Имя не указано"
                : claims.FindFirstValue(ClaimTypes.Sid);
        }
        public static class AuthOptions
        {
            public const string ISSUER = "ConnectorCenter"; // издатель токена
            public const string AUDIENCE = "ConnectorClient"; // потребитель токена
            const string KEY = "ExtremeUltraSuperLoooongSecretKey";   // ключ для шифрации
            public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        }
    }
}
