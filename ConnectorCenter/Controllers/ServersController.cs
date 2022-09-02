using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConnectorCenter.Data;
using ConnectorCenter.Views.Servers;
using ConnectorCore.Models.Connections;
using ConnectorCore.Models;
using ConnectorCenter.Views.DashBoard;
using Microsoft.AspNetCore.Authorization;

namespace ConnectorCenter.Controllers
{
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class ServersController : Controller
    {
        private readonly DataBaseContext _context;

        public ServersController(DataBaseContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return _context.Servers is null
                      ? View(new IndexModel(new List<Server>()))
                      : View(new IndexModel(await _context.Servers.Include("Connections").ToListAsync()));
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]        
        public async Task<IActionResult> Add(Server server)
        {
            if (ModelState.IsValid)
            {
                _context.Add(server);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Servers == null)
                return NotFound();
            Server? server = _context.Servers.Where(srv => srv.Id == id).FirstOrDefault();
            if (server == null)
                return NotFound();
            return View(new EditModel(server));
        }
        [HttpPost]
        public async Task<IActionResult> Edit(long id, Server server)
        {
            if (id != server.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(server);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServerExists(server.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> ChangeAcessMode(long? id)
        {
            if (id == null || _context.Servers == null)
                return NotFound();
            Server server = await _context.Servers.FindAsync(id) ?? null!;
            if (server == null)
                return NotFound();
            server.IsAvailable = !server.IsAvailable;
            _context.Update(server);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(long? id)
        {
            if (_context.Servers == null)
                return Problem("Entity set 'DataBaseContext.Servers'  is null.");
            Server? server = await _context.Servers.FindAsync(id);
            if (server != null)
                _context.Servers.Remove(server);            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ShowConnections(long id)
        {
            if (_context.Servers == null)
                return Problem("Entity set 'DataBaseContext.Servers'  is null.");
            Server? server = await _context.Servers
                .Include(conn => conn.Connections)
                    .ThenInclude(conn => conn.User)
                        .ThenInclude(conn => conn!.Credentials)
                .FirstOrDefaultAsync(serv => serv.Id == id);
            if (server != null)
                return View(new ShowConnectionsModel(server));
            return NotFound();
        }

        private bool ServerExists(long id)
        {
          return (_context.Servers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
