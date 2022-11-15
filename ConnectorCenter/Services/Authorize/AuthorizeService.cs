using ConnectorCenter.Data;
using ConnectorCore.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using ConnectorCenter.Models.Settings;
using System.Net;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ConnectorCore.Cryptography;

namespace ConnectorCenter.Services.Authorize
{
    public abstract class AuthorizeService
    {
        public static bool IsAuthorized(DataBaseContext dbContext, Credentials credentials, out AppUser? user)
        {
            List<AppUser> userList =
                dbContext.Users.Where(user =>
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
                if (!dbContext.Users.Where(usr => usr.Role == AppUser.AppRoles.Administrator).Any()
                    && credentials.Login == AppUser.GetDefault().Credentials!.Login
                    && credentials.Password == PasswordCryptography.GetUserPasswordHash(
                        AppUser.GetDefault().Credentials!.Login,
                        AppUser.GetDefault().Credentials!.Password))
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
        public static long GetUserId(HttpContext context)
        {
            long result;
            if (long.TryParse(context.User.FindFirstValue(ClaimTypes.Sid), out result))
                return result;
            else throw new Exception("Не удалось определить Id пользователя в запросе.");            
        }
        public static AppUser.AppRoles GetUserRole(HttpContext context)
        {
            object? role;
            if (Enum.TryParse(typeof(AppUser.AppRoles), context.User.FindFirstValue(ClaimTypes.GroupSid), true, out role))
                return (AppUser.AppRoles)role!;
            else throw new Exception("Не удалось определить роль пользователя в запросе.");
        }
        public static string GetUserName(HttpContext context)
        {
            return String.IsNullOrWhiteSpace(context.User.Identity?.Name)
                ? "Unknow"
                : context.User.Identity.Name;
        }
        public static IActionResult ForbiddenActionResult(ControllerBase controller, string backUrl)
        {
            return controller.RedirectToAction("Index", "Message", new RouteValueDictionary(
                            new
                            {
                                message = $"Нет прав для доступа к этой странице. Обратитесь к администратору.",
                                buttons = new Dictionary<string, string>()
                                {
                                    {"На главную",@"\dashboard" },
                                    {"Назад",$"{backUrl}" }
                                },
                                errorCode = 403
                            }));
        }
        public static AccessSettings GetAccessSettings(HttpContext context)
        {
            AppUser.AppRoles userRole = GetUserRole(context);
            return GetAccessSettings(userRole);
        }
        public static AccessSettings GetAccessSettings(AppUser.AppRoles? role)
        {
            switch (role)
            {
                case null:
                    throw new Exception("Ошибка при попытке проверить права доступа. Не указана роль.");
                case AppUser.AppRoles.User:
                    return ConnectorCenterApp.Instance.UserAccessSettings;
                case AppUser.AppRoles.Support:
                    return ConnectorCenterApp.Instance.SupportAccessSettings;
                case AppUser.AppRoles.Administrator:
                    return AccessSettings.GetAdminDefault();
                default:
                    throw new Exception("Ошибка при попытке проверить права доступа. Неопознанная роль.");
            }
        }
    }
}
