using ConnectorCenter.Data;
using ConnectorCore.Models;

namespace ConnectorCenter.Services.Authorize
{
    public abstract class AuthorizeService
    {
        public static bool IsAuthorized(DataBaseContext dbContext, Сredentials credentials, out AppUser? user)
        {
            List<AppUser> userList =
                dbContext.AppUsers.Where(user =>
                        user.Credentials.Login == credentials.Login
                        && user.Credentials.Password == credentials.Password).ToList();
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
    }
}
