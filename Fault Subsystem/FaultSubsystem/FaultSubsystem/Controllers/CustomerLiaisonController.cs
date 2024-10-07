using FaultSubsystem.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FaultSubsystem.Controllers
{
    public class CustomerLiaisonController : Controller
    {
        public IActionResult Dashboard()
        {
            var tiles = new List<TileModel> 
            {
                new TileModel {Title = "Fridge Allocation", Description = "Allocate fridges to customers", Action = "Dashboard", Controller = "CustomerLiaison"}
            };

            TempData["TilesList"] = JsonConvert.SerializeObject(tiles);

            return RedirectToAction("Dashboard", "Shared");
        }
    }
}
