using ConnectorCenter.Data;
using ConnectorCore.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ConnectorCore.Models.VisualModels.Interfaces;

namespace ConnectorCenter.Services.Authorize
{
    public abstract class AuthorizeService
    {
        public static bool IsAuthorized(DataBaseContext dbContext, Сredentials credentials, out AppUser? user)
        {
            List<AppUser> userList =
                dbContext.Users.Where(user =>
                        user.Credentials.Login == credentials.Login
                        && user.Credentials.Password == credentials.Password
                        && user.IsEnabled == true)
                .Include("Credentials")
                .ToList();
            if (userList.Any())
            {
                user = userList.SingleOrDefault();
                return true;
            }
            else
            {
                if (!dbContext.Users.Any()
                    && credentials.Login == AppUser.GetDefault().Credentials!.Login
                    && credentials.Password == AppUser.GetDefault().Credentials!.Password)
                {
                    user = AppUser.GetDefault();
                    return true;
                }
                else
                {
                    user = null;
                    return false;
                }
            }
        }
        public static List<Claim> GetUserClaims( AppUser user)
        {
            if(user is null) return new List<Claim>();
            return new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.GroupSid, user.Role.ToString())
            };
        }

        public static bool CompareHttpUserWithAppUser(ClaimsPrincipal claims, AppUser user)
        {
            if (claims.FindFirstValue(ClaimTypes.Sid) == user.Id.ToString() && claims.FindFirstValue(ClaimTypes.Name) == user.Name)
                return true;
            else return false;
        }
        public static AppUser.AppRoles? GetUserRole(HttpContext context)
        {
            object? role;
            if (Enum.TryParse(typeof(AppUser.AppRoles), context.User.FindFirstValue(ClaimTypes.GroupSid), true, out role))
                return (AppUser.AppRoles)role!;
            else return null;
        }
        public static string GetUserName(HttpContext context)
        {
            return String.IsNullOrWhiteSpace(context.User.Identity?.Name)
                ? "Unknow"
                : context.User.Identity.Name;
        }
    }
}
