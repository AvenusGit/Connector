using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ConnectorCenter.Views.VisualScheme;
using ConnectorCore.Models.VisualModels.Interfaces;
using ConnectorCenter.Data;
using static System.Net.WebRequestMethods;
using Microsoft.EntityFrameworkCore;
using ConnectorCore.Models;
using ConnectorCore.Models.VisualModels;
using Microsoft.AspNetCore.Authorization;

namespace ConnectorCenter.Controllers
{
    public class VisualSchemeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataBaseContext _dataBaseContext;

        public VisualSchemeController(ILogger<HomeController> logger, DataBaseContext context)
        {
            _logger = logger;
            _dataBaseContext = context;
        }
        public async Task<IActionResult> GenerateCss()
        {
            HttpContext.Response.ContentType = "text/css";
            long userId;
            if(long.TryParse(HttpContext.User.FindFirstValue(ClaimTypes.Sid), out userId))
            {
                AppUser currentUser =
                    _dataBaseContext.Users
                        .Where(user => user.Id == userId)
                        .SingleOrDefaultAsync()
                        .Result;
                if(currentUser is not null)
                    if(currentUser.VisualScheme is not null)
                        return View(new GenerateCssModel(currentUser.VisualScheme));
            }
            return View(new GenerateCssModel(VisualScheme.GetDefaultVisualScheme()));


        }
    }
}
