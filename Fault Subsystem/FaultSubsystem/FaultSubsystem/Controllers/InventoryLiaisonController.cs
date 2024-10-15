using FaultSubsystem.Data;
using FaultSubsystem.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FaultSubsystem.Controllers
{
    public class InventoryLiaisonController : Controller
    {
        private readonly ApplicationDbContext _dBContext;

        public InventoryLiaisonController(ApplicationDbContext context)
        {
            _dBContext = context;
        }

        public IActionResult Dashboard()
        {
            var tiles = new List<TileModel>
            {
                new TileModel {Title = "Suppliers", Description = "View and edit supplier information.", Action = "Dashboard", Controller = "CustomerLiaison"},
                new TileModel {Title = "View Customers", Description = "View all customers and their information.", Action = "Dashboard", Controller = "CustomerLiaison"}
            };

            TempData["TilesList"] = JsonConvert.SerializeObject(tiles);

            return RedirectToAction("Dashboard", "Shared");
        }
    }
}
