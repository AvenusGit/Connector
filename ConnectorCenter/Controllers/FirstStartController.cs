using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ConnectorCenter.Data;
using ConnectorCenter.Services.Authorize;
using ConnectorCenter.Views.FirstStart;
using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using ConnectorCore.Models.VisualModels.Interfaces;
using ConnectorCore.Models.VisualModels;

namespace ConnectorCenter.Controllers
{
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class FirstStartController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataBaseContext _dataBaseContext;

        public FirstStartController(ILogger<HomeController> logger, DataBaseContext context)
        {
            _logger = logger;
            _dataBaseContext = context;
        }

        public IActionResult Index()
        {
            return View(new IndexViewModel(_dataBaseContext));
        }
        [HttpPost]
        public IActionResult CreateFirstUser(string username, Сredentials credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            AppUser newUser = new AppUser()
            {
                Name = username,
                IsEnabled = true,
                Credentials = credentials,
                Role = AppUser.AppRoles.Administrator,
                Connections = new List<Connection>(),
                UserSettings = UserSettings.GetDefault(),
                VisualScheme = VisualScheme.GetDefaultVisualScheme()
            };
            _dataBaseContext.Users.Add(newUser);
            _dataBaseContext.SaveChanges();
            CookieAuthorizeService.SignOut(HttpContext);
            return RedirectToAction("Index","Login");
        }
    }
}
