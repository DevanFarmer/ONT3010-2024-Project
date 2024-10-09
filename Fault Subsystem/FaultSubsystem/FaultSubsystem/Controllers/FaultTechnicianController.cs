using FaultSubsystem.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FaultSubsystem.Controllers
{
    public class FaultTechnicianController : Controller
    {
        public IActionResult Dashboard()
        {
            var tiles = new List<TileModel>
            {
                new TileModel {Title = "Manage Faulty Fridges", Description = "Manage your assigned faulty fridges.", Action = "ReportFault", Controller = "FaultTechnician"},
                new TileModel {Title = "View All Faults", Description = "View all reports of faulty fridges.", Action = "Dashboard", Controller = "FaultTechnician"}
            };

            TempData["TilesList"] = JsonConvert.SerializeObject(tiles);

            return RedirectToAction(nameof(SharedController.Dashboard), nameof(SharedController));
        }
    }
}
