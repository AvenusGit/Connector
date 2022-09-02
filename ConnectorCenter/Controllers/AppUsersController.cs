using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConnectorCenter.Data;
using ConnectorCenter.Views.AppUsers;
using ConnectorCore.Models;
using ConnectorCore.Models.VisualModels;
using ConnectorCenter.Services.Authorize;
using ConnectorCore.Models.Connections;

namespace ConnectorCenter.Controllers
{
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class AppUsersController : Controller
    {
        private readonly DataBaseContext _context;

        public AppUsersController(DataBaseContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return _context.Users is null
              ? View(new IndexModel(new List<AppUser>()))
              : View(new IndexModel(await _context.Users
                    .Include(user =>user.Credentials)
                    .Include(user => user.Connections)
                    .ToListAsync()));
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                appUser.VisualScheme = VisualScheme.GetDefaultVisualScheme();
                _context.Users.Add(appUser);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index","AppUsers");
            }
            return BadRequest();
        }
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Users == null)
                return NotFound();

            AppUser? appUser = await _context.Users.Include(usr => usr.Credentials).Where(usr => usr.Id == id).FirstOrDefaultAsync();
            if (appUser == null)
                return NotFound();
            return View(new EditModel(appUser));
        }
        [HttpPost]
        public async Task<IActionResult> Edit(AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                if (AppUserExists(appUser.Id))
                {
                    _context.Update(appUser);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else return NotFound();                
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Users == null)
                return NotFound();

            AppUser? appUser = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appUser != null)
            {
                _context.Users.Remove(appUser);
                await _context.SaveChangesAsync();
                if(AuthorizeService.CompareHttpUserWithAppUser(HttpContext.User, appUser))
                {
                    CookieAuthorizeService.SignOut(HttpContext);
                    return RedirectToAction("Index", "Login", new RouteValueDictionary(
                                new { message = "Ваш пользователь удален." }));
                }
                return RedirectToAction("Index");
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> ChangeEnableMode(long? id)
        {
            if (id == null || _context.Users == null)
                return NotFound();
            AppUser? user = await _context.Users.FindAsync(id) ?? null!;
            if (user == null)
                return NotFound();
            user.IsEnabled = !user.IsEnabled;
            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ShowConnections(long? userId)
        {
            if (!userId.HasValue) return BadRequest();
            if (_context.Users == null)
                return Problem("Entity set 'DataBaseContext.Users'  is null.");
            AppUser? user = await _context.Users
                .Include(usr => usr.Connections)
                    .ThenInclude(conn => conn.User)
                        .ThenInclude(usr => usr!.Credentials)
                .FirstOrDefaultAsync(usr => usr.Id == userId);
            if (user != null)
                return View(new ShowConnectionsModel(user));
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> AddConnections(long? userId)
        {
            if (!userId.HasValue)
                return BadRequest();
            AppUser? user = await _context.Users.FindAsync(userId);
            List<Server> servers = _context.Servers
                .Include(srv => srv.Connections)
                    .ThenInclude(conn => conn.User)
                        .ThenInclude(usr => usr!.Credentials)
                .ToList();
            if(user is null) 
                return NotFound();
            if (servers is null)
                servers = new List<Server>();
            return View(new AddConnectionsModel(servers, user));
        }
        [HttpPost]
        public async Task<IActionResult> AddConnections(long? connectionId, long? userId)
        {
            if (!connectionId.HasValue || !userId.HasValue)
                return BadRequest();
            AppUser? user = await _context.Users
                .Include(usr => usr.Connections)
                .Where(usr => usr.Id == userId)
                .FirstOrDefaultAsync();
            Connection? connection = await _context.Connections.FindAsync(connectionId);
            user.Connections.Add(connection);
            _context.Update(user);
            await _context.SaveChangesAsync();
            List<Server> servers = _context.Servers
                .Include(srv => srv.Connections)
                    .ThenInclude(conn => conn.User)
                        .ThenInclude(usr => usr!.Credentials)
                .ToList();
            return View("AddConnections", new AddConnectionsModel(servers, user));
        }

        [HttpPost]
        public async Task<IActionResult> DropConnectionOnAddConnectionList(long? connectionId, long? userId)
        {
            if (!connectionId.HasValue || !userId.HasValue)
                return BadRequest();

            AppUser? user = await _context.Users
                .Include(usr => usr.Connections)
                .Where(usr => usr.Id == userId)
                .FirstOrDefaultAsync();

            if (user is null) return NotFound();

            await DropConnection(connectionId.Value, user);
            List<Server> servers = _context.Servers
                .Include(srv => srv.Connections)
                    .ThenInclude(conn => conn.User)
                        .ThenInclude(usr => usr!.Credentials)
                .ToList();
            return View("AddConnections", new AddConnectionsModel(servers, user));
        }

        [HttpPost]
        public async Task<IActionResult> DropConnectionOnConnectionList(long? connectionId, long? userId)
        {
            if (!connectionId.HasValue || !userId.HasValue)
                return BadRequest();
            AppUser? user = await _context.Users
                .Include(usr => usr.Connections)
                .Where(usr => usr.Id == userId)
                .FirstOrDefaultAsync();

            if (user is null) return NotFound();
            await DropConnection(connectionId.Value, user);
            return View("ShowConnections", new ShowConnectionsModel(user));
        }

        private async Task DropConnection(long connectionId, AppUser user)
        {
            Connection? connection = await _context.Connections.FindAsync(connectionId);
            if (connection is null) return;
            user.Connections.Remove(connection);
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        private bool AppUserExists(long id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
