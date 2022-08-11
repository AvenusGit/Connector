using Microsoft.AspNetCore.Mvc;
using ConnectorCenter.Views.Login;

namespace ConnectorCenter.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index(string message = null!)
        {
            return View(new IndexModel(message));
        }        
    }
}
