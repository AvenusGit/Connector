using ConnectorCenter.Data;
using ConnectorCenter.Views.Js;
using ConnectorCore.Models.VisualModels;
using ConnectorCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ConnectorCenter.Controllers
{
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class JsController : Controller
    {
        private readonly DataBaseContext _dataBaseContext;
        public JsController(DataBaseContext context)
        {
            _dataBaseContext = context;
        }
        /// <summary>
        /// Запрос на генерацию скрипта статистики
        /// </summary>
        /// <returns>JavaScript с цветами пользовательской схемы.</returns>
        [HttpGet]
        public async Task<IActionResult> StatisticJS()
        {
            HttpContext.Response.ContentType = "text/javascript";
            long userId;
            if (long.TryParse(HttpContext.User.FindFirstValue(ClaimTypes.Sid), out userId))
            {
                // этот запрос не логируется и не учитывается в статистике
                try
                {
                    _dataBaseContext.Users
                        .Where(user => user.Id == userId)
                        .Load();
                    AppUser? currentUser = await _dataBaseContext.Users
                        .FirstOrDefaultAsync(user => user.Id == userId);
                    if (currentUser is not null)
                        if (currentUser.VisualScheme is not null)
                            return View(new JsModel(currentUser.VisualScheme));
                }
                catch
                {
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Внутренняя ошибка обработки запроса на JS статистики.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }
            }
            return View(new JsModel(VisualScheme.GetDefaultVisualScheme()));
        }
    }
}
