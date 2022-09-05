using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConnectorCenter.Data;
using ConnectorCore.Models;
using ConnectorCore.Models.Connections;
using ConnectorCenter.Views.AppUserGroups;

namespace ConnectorCenter.Controllers
{
    public class AppUserGroupsController : Controller
    {
        private readonly DataBaseContext _context;

        public AppUserGroupsController(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return _context.UserGroups is null
                  ? View(new IndexModel(new List<AppUserGroup>()))
                  : View(new IndexModel(await _context.UserGroups
                        .Include(gr => gr.GroupConnections)
                        .Include(gr => gr.Users)
                        .ToListAsync()));
        }

        public IActionResult Add()
        {
            return View(new AddAppUserGroupsModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(AppUserGroup appUserGroup)
        {
            if (ModelState.IsValid)
            {
                _context.UserGroups.Add(appUserGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.UserGroups == null)
                return NotFound();

            AppUserGroup? appUserGroup = await _context.UserGroups.FindAsync(id);
            if (appUserGroup == null)
                return NotFound();
            return View(new EditUserGroupsModel(appUserGroup));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AppUserGroup appUserGroup)
        {
            if (ModelState.IsValid)
            {
                _context.Update(appUserGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            if (_context.UserGroups == null)
                   return Problem("Entity set 'DataBaseContext.UserGroups'  is null.");
            AppUserGroup? appUserGroup = await _context.UserGroups
                .Include(gr => gr.Users)
                .Include(gr => gr.GroupConnections)
                .FirstOrDefaultAsync(usr => usr.Id == id);
            if (appUserGroup != null)
            {
                appUserGroup.GroupConnections.Clear();
                appUserGroup.Users.Clear(); // очистка пользователей и подключений, чтобы они не были удалены каскадно
                _context.Update(appUserGroup);
                await _context.SaveChangesAsync();
                _context.UserGroups.Remove(appUserGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }                
            else return NotFound();          
        }

        [HttpGet]
        public async Task<IActionResult> ShowConnections(long? groupId)
        {
            if (!groupId.HasValue) return BadRequest();
            if (_context.UserGroups == null)
                return Problem("Entity set 'DataBaseContext.UserGroups'  is null.");
            AppUserGroup? userGroup = await _context.UserGroups
                .Include(usr => usr.GroupConnections)
                    .ThenInclude(conn => conn.User)
                        .ThenInclude(usr => usr!.Credentials)
                .FirstOrDefaultAsync(usr => usr.Id == groupId);
            if (userGroup != null)
                return View(new ShowConnectionsModel(userGroup));
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> AddConnections(long? groupId)
        {
            if (!groupId.HasValue)
                return BadRequest();
            AppUserGroup? group = await _context.UserGroups.FindAsync(groupId);
            List<Server> servers = _context.Servers
                .Include(srv => srv.Connections)
                    .ThenInclude(conn => conn.User)
                        .ThenInclude(usr => usr!.Credentials)
                .ToList();
            if (group is null)
                return NotFound();
            if (servers is null)
                servers = new List<Server>();
            return View(new AddConnectionsModel(servers, group));
        }
        [HttpPost]
        public async Task<IActionResult> AddConnections(long? connectionId, long? groupId)
        {
            if (!connectionId.HasValue || !groupId.HasValue)
                return BadRequest();
            AppUserGroup? group = await _context.UserGroups
                .Include(usr => usr.GroupConnections)
                .Where(usr => usr.Id == groupId)
                .FirstOrDefaultAsync();
            Connection? connection = await _context.Connections.FindAsync(connectionId);
            group.GroupConnections.Add(connection);
            _context.Update(group);
            await _context.SaveChangesAsync();
            List<Server> servers = _context.Servers
                .Include(srv => srv.Connections)
                    .ThenInclude(conn => conn.User)
                        .ThenInclude(usr => usr!.Credentials)
                .ToList();
            return View("AddConnections", new AddConnectionsModel(servers, group));
        }

        [HttpPost]
        public async Task<IActionResult> DropConnectionOnAddConnectionList(long? connectionId, long? groupId)
        {
            if (!connectionId.HasValue || !groupId.HasValue)
                return BadRequest();

            AppUserGroup? group = await _context.UserGroups
                .Include(usr => usr.GroupConnections)
                .Where(usr => usr.Id == groupId)
                .FirstOrDefaultAsync();

            if (group is null) return NotFound();

            await DropConnection(connectionId.Value, group);
            List<Server> servers = _context.Servers
                .Include(srv => srv.Connections)
                    .ThenInclude(conn => conn.User)
                        .ThenInclude(usr => usr!.Credentials)
                .ToList();
            return View("AddConnections", new AddConnectionsModel(servers, group));
        }

        [HttpPost]
        public async Task<IActionResult> DropConnectionOnConnectionList(long? connectionId, long? groupId)
        {
            if (!connectionId.HasValue || !groupId.HasValue)
                return BadRequest();
            AppUserGroup? group = await _context.UserGroups
                .Include(usr => usr.GroupConnections)
                .Where(usr => usr.Id == groupId)
                .FirstOrDefaultAsync();

            if (group is null) return NotFound();
            await DropConnection(connectionId.Value, group);
            return View("ShowConnections", new ShowConnectionsModel(group));
        }

        [HttpGet]
        public async Task<IActionResult> ShowUsers(long? groupId)
        {
            if (!groupId.HasValue) return BadRequest();
            if (_context.UserGroups == null)
                return Problem("Entity set 'DataBaseContext.UserGroups'  is null.");
            AppUserGroup? userGroup = await _context.UserGroups
                .Include(gr => gr.Users)
                    .ThenInclude(usr => usr.Credentials)
                .FirstOrDefaultAsync(usr => usr.Id == groupId);
            if (userGroup != null)
                return View(new ShowUsersModel(userGroup));
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> AddUser(long? groupId)
        {
            if (!groupId.HasValue)
                return BadRequest();
            AppUserGroup? group = await _context.UserGroups
                .Include(gr => gr.Users)
                .FirstOrDefaultAsync(gr => gr.Id == groupId);
            List<AppUser> users = _context.Users
                .Include(usr => usr.Credentials)
                .ToList();
            if (group is null)
                return NotFound();
            if (users is null)
                users = new List<AppUser>();
            return View(new AddUserModel(users, group));
        }
        [HttpPost]
        public async Task<IActionResult> AddUser(long? userId, long? groupId)
        {
            if (!userId.HasValue || !groupId.HasValue)
                return BadRequest();
            AppUserGroup? group = await _context.UserGroups
                .Include(usr => usr.Users)
                .Where(usr => usr.Id == groupId)
                .FirstOrDefaultAsync();
            AppUser? user = await _context.Users.FindAsync(userId);
            group.Users.Add(user);
            _context.Update(group);
            await _context.SaveChangesAsync();
            List<AppUser> users = _context.Users
                .Include(usr => usr.Credentials)
                .ToList();
            return View("AddUser", new AddUserModel(users, group));
        }

        [HttpPost]
        public async Task<IActionResult> DropUserOnAddUserList(long? userId, long? groupId)
        {
            if (!userId.HasValue || !groupId.HasValue)
                return BadRequest();

            AppUserGroup? group = await _context.UserGroups
                .Include(usr => usr.Users)
                .Where(usr => usr.Id == groupId)
                .FirstOrDefaultAsync();

            if (group is null) return NotFound();

            await DropUser(userId.Value, group);
            List<AppUser> users = _context.Users
                .Include(srv => srv.Credentials)
                .ToList();
            return View("AddUser", new AddUserModel(users, group));
        }

        [HttpPost]
        public async Task<IActionResult> DropUserOnUserList(long? userId, long? groupId)
        {
            if (!userId.HasValue || !groupId.HasValue)
                return BadRequest();
            AppUserGroup? group = await _context.UserGroups
                .Include(usr => usr.Users)
                .Where(usr => usr.Id == groupId)
                .FirstOrDefaultAsync();

            if (group is null) return NotFound();
            await DropUser(userId.Value, group);
            return View("ShowUsers", new ShowUsersModel(group));
        }

        private async Task DropConnection(long connectionId, AppUserGroup group)
        {
            Connection? connection = await _context.Connections.FindAsync(connectionId);
            if (connection is null) return;
            group.GroupConnections.Remove(connection);
            _context.Update(group);
            await _context.SaveChangesAsync();
        }
        private async Task DropUser(long userId, AppUserGroup group)
        {
            AppUser? user = await _context.Users.FindAsync(userId);
            if (user is null) return;
            group.Users.Remove(user);
            _context.Update(group);
            await _context.SaveChangesAsync();
        }
    }
}
