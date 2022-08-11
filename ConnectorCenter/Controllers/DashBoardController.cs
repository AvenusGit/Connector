using Microsoft.AspNetCore.Mvc;

namespace ConnectorCenter.Controllers
{
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
