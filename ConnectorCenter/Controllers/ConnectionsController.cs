using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConnectorCenter.Views.Connections;
using Microsoft.EntityFrameworkCore;
using ConnectorCenter.Data;
using ConnectorCore.Models.Connections;
using ConnectorCore.Models;
using Microsoft.AspNetCore.Authorization;

namespace ConnectorCenter.Controllers
{
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class ConnectionsController : Controller
    {
        private readonly DataBaseContext _context;

        public ConnectionsController(DataBaseContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Add(long serverId)
        {
            Server? server = await _context.Servers.FindAsync(serverId);
            if (server is not null)
                return View(new AddModel(server));
            else
                return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Add(long serverId, Connection connection)
        {
            if (ModelState.IsValid)
            {
                Server? server = await _context.Servers.FindAsync(serverId);
                if (server is not null)
                {
                    connection.Server = server;
                    server.Connections.Add(connection);
                    _context.Update(server);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ShowConnections","Servers", new RouteValueDictionary(
                                new { id = connection.Server.Id}));
                }                
            }
            return View(connection);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(long connectionId)
        {
            if (connectionId == null || _context.Connections == null)
            {
                return NotFound();
            }

            Connection? connection = _context.Connections
                .Include(conn => conn.Server)
                .Include(conn => conn.User)
                    .ThenInclude(user => user!.Credentials)
                .Where(conn => conn.Id == connectionId)
                .FirstOrDefault();
            if (connection == null)
            {
                return NotFound();
            }
            return View(new EditModel(connection));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(long serverId, Connection connection)
        {
            if (!ConnectionExists(connection.Id))
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(connection);
                await _context.SaveChangesAsync();
                return RedirectToAction("ShowConnections", "Servers", new RouteValueDictionary(
                                new{id = serverId }));
            }
            else return BadRequest();            
        }

        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Connections == null)
                return NotFound();
            
            Connection? connection = await _context.Connections
                .Include(srv => srv.Server)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (connection == null)
                return NotFound();
            else
            {
                _context.Connections.Remove(connection);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ShowConnections", "Servers", new RouteValueDictionary(
                                new { id = connection.Server.Id }));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeAcessMode(long? id)
        {
            if (id == null || _context.Connections == null)
                return NotFound();
            Connection? connection = _context.Connections
                .Include(conn => conn.Server)
                .Where(conn => conn.Id == id)
                .FirstOrDefault();
            if (connection == null)
                return NotFound();
            connection.IsAvailable = !connection.IsAvailable;
            _context.Update(connection);
            await _context.SaveChangesAsync();
            return RedirectToAction("ShowConnections", "Servers", new RouteValueDictionary(
                                new
                                {
                                    id = connection.Server.Id
                                }));
        }

        private bool ConnectionExists(long id)
        {
          return (_context.Connections?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
