using Microsoft.AspNetCore.Mvc;
using FaultSubsystem.Models.Shared;
using Newtonsoft.Json;
using FaultSubsystem.Data;
using FaultSubsystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FaultSubsystem.Controllers
{
    public class AdministratorController : Controller
    {
        private readonly ApplicationDbContext _dBContext;

        public AdministratorController(ApplicationDbContext context)
        {
            _dBContext = context;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            var tiles = new List<TileModel>
            {
                new TileModel {Title = "Customers", Description = "Manage Customers", Action = "Dashboard", Controller = "CustomerLiaison"},
                new TileModel {Title = "Employees", Description = "Manage Employees", Action = "Dashboard", Controller = "Employee"},
                new TileModel {Title = "Locations", Description = "Manage Locations", Action = "ViewLocations", Controller = "Administrator"},
                new TileModel {Title = "Roles", Description = "View, Add and Edit roles.", Action = "ViewRoles", Controller = "Role"},
            };

            TempData["TilesList"] = JsonConvert.SerializeObject(tiles);

            return RedirectToAction("Dashboard", "Shared");
        }

        public IActionResult ViewRoles()
        {
            return RedirectToAction("ViewRoles", "Role");
        }

        #region Manage Locations
        // View all locations
        public async Task<IActionResult> ViewLocations()
        {
            var locations = await _dBContext.Location.ToListAsync();
            return View(locations);
        }

        // GET: Add location
        public IActionResult AddLocation()
        {
            return View();
        }

        // POST: Add location
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLocation(Location location)
        {
            if (ModelState.IsValid)
            {
                _dBContext.Location.Add(location);
                await _dBContext.SaveChangesAsync();
                return RedirectToAction(nameof(ViewLocations));
            }
            return View(location);
        }

        // GET: Edit location
        public async Task<IActionResult> EditLocation(int id)
        {
            var location = await _dBContext.Location.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }

        // POST: Edit location
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLocation(int id, Location location)
        {
            if (id != location.LocationID)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _dBContext.Location.Update(location);
                await _dBContext.SaveChangesAsync();
                return RedirectToAction(nameof(ViewLocations));
            }
            return View(location);
        }
        #endregion
    }
}
