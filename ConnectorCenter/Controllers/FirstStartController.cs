using Microsoft.AspNetCore.Mvc;
using ConnectorCenter.Data;
using ConnectorCenter.Views.FirstStart;

namespace ConnectorCenter.Controllers
{
    public class FirstStartController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public FirstStartController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        //public IActionResult FirstStart()
        //{
        //    //
        //}
    }
}
