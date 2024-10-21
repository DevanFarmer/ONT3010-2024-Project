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
                new TileModel {Title = "Customers", Description = "Manage Customers", Action = "ViewCustomers", Controller = "CustomerLiaison"},
                new TileModel {Title = "Employees", Description = "Manage Employees", Action = "ViewEmployees", Controller = "Employee"},
                new TileModel {Title = "Locations", Description = "Manage Locations", Action = "ViewLocations", Controller = "Administrator"},
                new TileModel {Title = "Roles", Description = "Manage Roles.", Action = "ViewRoles", Controller = "Role"},
                new TileModel {Title = "Fault Statuses", Description = "Manage Fault Statuses.", Action = "ViewFaultStatuses", Controller = "Administrator"}
            };

            TempData["TilesList"] = JsonConvert.SerializeObject(tiles);

            return RedirectToAction("Dashboard", "Shared");
        }

        public IActionResult ViewRoles()
        {
            return RedirectToAction("ViewRoles", "Role");
        }

        #region Manage Locations
        public async Task<IActionResult> ViewLocations()
        {
            var locations = await _dBContext.Location.ToListAsync();
            return View(locations);
        }

        public IActionResult AddLocation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLocation(Location location)
        {
            if (ModelState.IsValid)
            {
                // Increment ID
                var maxLocations = await _dBContext.Location.MaxAsync(l => (int?)l.LocationID) ?? 0;

                var newLocationID = maxLocations + 1;

                location.LocationID = newLocationID;

                _dBContext.Location.Add(location);
                await _dBContext.SaveChangesAsync();
                return RedirectToAction(nameof(ViewLocations));
            }
            return View(location);
        }

        public async Task<IActionResult> EditLocation(int id)
        {
            var location = await _dBContext.Location.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }

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

        #region Manage Fault Statuses
        public async Task<IActionResult> ViewFaultStatuses()
        {
            var faultStatuses = await _dBContext.FaultStatus.ToListAsync();
            return View(faultStatuses);
        }

        public IActionResult AddFaultStatus()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFaultStatus(FaultStatus model)
        {
            if (ModelState.IsValid)
            {
                // Increment ID
                var maxStatuses = await _dBContext.FaultStatus.MaxAsync(fs => (int?)fs.FaultStatusID) ?? 0;

                var newStatusID = maxStatuses + 1;

                model.FaultStatusID = newStatusID;

                _dBContext.FaultStatus.Add(model);
                await _dBContext.SaveChangesAsync();
                return RedirectToAction(nameof(ViewFaultStatuses));
            }

            return View(model);
        }

        public async Task<IActionResult> EditFaultStatus(int id)
        {
            var faultStatus = await _dBContext.FaultStatus.FindAsync(id);
            if (faultStatus == null)
            {
                return NotFound();
            }

            return View(faultStatus);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFaultStatus(int id, FaultStatus model)
        {
            Console.WriteLine($"ID: {id}");
            Console.WriteLine($"MID: {model?.FaultStatusID}");
            if (id != model.FaultStatusID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _dBContext.Update(model);
                await _dBContext.SaveChangesAsync();
                return RedirectToAction(nameof(ViewFaultStatuses));
            }

            return View(model);
        }
        #endregion
    }
}
