using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ConnectorCenter.Views.VisualScheme;
using ConnectorCore.Models.VisualModels.Interfaces;
using ConnectorCenter.Data;
using static System.Net.WebRequestMethods;
using Microsoft.EntityFrameworkCore;
using ConnectorCore.Models;
using ConnectorCore.Models.VisualModels;
using Microsoft.AspNetCore.Authorization;

namespace ConnectorCenter.Controllers
{
    /// <summary>
    /// Контроллер для генерации CSS всех страниц
    /// </summary>
    [AllowAnonymous]
    public class VisualSchemeController : Controller
    {        
        private readonly DataBaseContext _dataBaseContext;
        public VisualSchemeController(DataBaseContext context)
        {
            _dataBaseContext = context;
        }
        /// <summary>
        /// Запрос на генерацию CSS
        /// </summary>
        /// <returns>CSS с цветами визуальной схемы пользователя, если он аутентифицирован, иначе - страндартная цветовая схема</returns>
        [HttpGet]
        public async Task<IActionResult> GenerateCss()
        {
            HttpContext.Response.ContentType = "text/css";
            long userId;
            if(long.TryParse(HttpContext.User.FindFirstValue(ClaimTypes.Sid), out userId))
            {
                // этот запрос не логируется и не учитывается в статистике ввиду его частоты
                try
                {
                    _dataBaseContext.Users
                        .Where(user => user.Id == userId)
                        .Load();
                    AppUser? currentUser = await _dataBaseContext.Users                        
                        .FirstOrDefaultAsync(user => user.Id == userId);
                    if (currentUser is not null)
                        if (currentUser.VisualScheme is not null)
                            return View(new GenerateCssModel(currentUser.VisualScheme));
                }
                catch 
                {
                    return RedirectToAction("Index", "Message", new RouteValueDictionary(
                        new
                        {
                            message = "Внутренняя ошибка обработки запроса на CSS.",
                            buttons = new Dictionary<string, string>()
                            {
                                {"На главную",@"\dashboard" },
                                {"К логам",@"\logs" }
                            },
                            errorCode = 500
                        }));
                }                
            }
            return View(new GenerateCssModel(VisualScheme.GetDefaultVisualScheme()));          
        }
    }
}
