using ConnectorCenter.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ConnectorCenter.Views.Login;

namespace ConnectorCenter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "DashBoard");
            else
                return RedirectToAction("Index","Login");
        }
    }
}