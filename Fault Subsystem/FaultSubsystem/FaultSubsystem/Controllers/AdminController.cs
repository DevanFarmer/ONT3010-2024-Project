using Microsoft.AspNetCore.Mvc;
using FaultSubsystem.Models.Shared;
using Newtonsoft.Json;

namespace FaultSubsystem.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult Dashboard()
        {
            var tiles = new List<TileModel>
            {
                new TileModel {Title = "Roles", Description = "Add and Edit roles.", Action = "ViewRoles", Controller = "Role"},
                new TileModel {Title = "Employees", Description = "Manage Employees", Action = "ViewEmployees", Controller = "Admin"},
                new TileModel {Title = "Tile 3", Description = "Description", Action = "Dashboard", Controller = "Admin"}
            };

            TempData["TilesList"] = JsonConvert.SerializeObject(tiles);

            return RedirectToAction("Dashboard", "Shared");
        }

        public IActionResult ViewRoles()
        {
            return RedirectToAction("ViewRoles", "Role");
        }

        public IActionResult ViewEmployees()
        {
            return View();
        }

        public IActionResult EmployeeDetails()
        {
            return View();
        }
    }
}
