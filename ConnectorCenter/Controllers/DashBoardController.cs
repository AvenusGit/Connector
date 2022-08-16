using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ConnectorCenter.Data;
using ConnectorCenter.Views.FirstStart;
using ConnectorCore.Models;

namespace ConnectorCenter.Controllers
{
    [Authorize(Policy = "Cookies")]
    public class DashBoardController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataBaseContext _dataBaseContext;

        public DashBoardController(ILogger<HomeController> logger, DataBaseContext context)
        {
            _logger = logger;
            _dataBaseContext = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
