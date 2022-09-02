using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConnectorCenter.Data;
using ConnectorCore.Models;
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
            AppUserGroup? appUserGroup = await _context.UserGroups.FindAsync(id);
            if (appUserGroup != null)
            {
                _context.UserGroups.Remove(appUserGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }                
            else return NotFound();          
        }

        private bool AppUserGroupExists(long id)
        {
          return (_context.UserGroups?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
