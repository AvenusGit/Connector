using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ConnectorCenter.Data;
using ConnectorCenter.Views.FirstStart;
using ConnectorCore.Models;

namespace ConnectorCenter.Controllers
{
    [Authorize]
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
            return View();
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
                Credentials = credentials,
                Role = AppUser.AppRoles.Administrator,
                Connections = new List<Connection>(),
                UserSettings = UserSettings.GetDefault()
            };
            _dataBaseContext.AppUsers.Add(newUser);
            _dataBaseContext.SaveChangesAsync();
            return RedirectToAction("Index","Login");

        }
    }
}
