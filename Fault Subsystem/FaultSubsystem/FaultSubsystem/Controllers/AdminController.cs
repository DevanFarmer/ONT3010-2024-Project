using Microsoft.AspNetCore.Mvc;
using FaultSubsystem.Models.Shared;

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
                new TileModel {Title = "Tile 2", Description = "Description", Action = "Dashboard", Controller = "Admin"},
                new TileModel {Title = "Tile 3", Description = "Description", Action = "Dashboard", Controller = "Admin"}
            };

            return View(tiles);
        }

        public IActionResult ViewRoles()
        {
            return RedirectToAction("ViewRoles", "Role");
        }
    }
}
