using FaultSubsystem.Models.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FaultSubsystem.Controllers
{
    public class CustomerController : Controller
    {
        [HttpGet]
        public IActionResult Dashboard()
        {
            var tiles = new List<TileModel>
            {
                new TileModel {Title = "Faults", Description = "Report or View Faults.", Action = "ViewFaults", Controller = "Role"},
                new TileModel {Title = "Tile 2", Description = "Description", Action = "Dashboard", Controller = "Admin"},
                new TileModel {Title = "Tile 3", Description = "Description", Action = "Dashboard", Controller = "Admin"}
            };

            return View(tiles);
        }
    }
}
