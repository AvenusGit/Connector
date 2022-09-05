using Microsoft.AspNetCore.Mvc;
using ConnectorCenter.Views.Message;

namespace ConnectorCenter.Controllers
{
    public class MessageController : Controller
    {
        public IActionResult Index(string message, Dictionary<string,string> buttons)
        {
            return View(new IndexModel(message, buttons));
        }
    }
}
