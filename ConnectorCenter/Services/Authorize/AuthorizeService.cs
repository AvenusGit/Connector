using ConnectorCenter.Data;
using ConnectorCore.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ConnectorCenter.Services.Authorize
{
    public abstract class AuthorizeService
    {
        public static bool IsAuthorized(DataBaseContext dbContext, Сredentials credentials, out AppUser? user)
        {
            List<AppUser> userList =
                dbContext.AppUsers.Where(user =>
                        user.Credentials.Login == credentials.Login
                        && user.Credentials.Password == credentials.Password)
                .Include("Credentials")
                .ToList();
            if (userList.Any())
            {
                user = userList.SingleOrDefault();
                return true;
            }
            else
            {
                if (!dbContext.AppUsers.Any()
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
    }
}
