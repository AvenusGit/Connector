using Microsoft.AspNetCore.Mvc;
using ConnectorCenter.Views.Login;
using Microsoft.AspNetCore.Authorization;
using ConnectorCore.Models;
using ConnectorCenter.Data;
using ConnectorCenter.Services.Authorize;

namespace ConnectorCenter.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataBaseContext _dataBaseContext;

        public LoginController(ILogger<HomeController> logger, DataBaseContext context)
        {
            _logger = logger;
            _dataBaseContext = context;
        }

        [HttpGet]
        public IActionResult Index(string? message)
        {
            return View(new IndexModel(message));
        }

        [HttpPost]
        public IActionResult Authorize(Сredentials сredentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            AppUser user;
            if (AuthorizeService.IsAuthorized(_dataBaseContext, сredentials, out user))
            {
                CookieAuthorizeService.SignIn(HttpContext, сredentials);
                return RedirectToAction("Index", "DashBoard");
            }
            else
            {
                return View("Index", new IndexModel("Неверный логин/пароль"));
            }
        }
    }
}
