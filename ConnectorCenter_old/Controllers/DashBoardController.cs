using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ConnectorCenter.Data;
using ConnectorCenter.Views.FirstStart;
using ConnectorCore.Models;

namespace ConnectorCenter.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
