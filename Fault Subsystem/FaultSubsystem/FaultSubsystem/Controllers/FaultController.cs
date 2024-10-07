using FaultSubsystem.Data;
using FaultSubsystem.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FaultSubsystem.Controllers
{
    public class FaultController : Controller
    {
        private readonly ApplicationDbContext _dBContext;

        public FaultController(ApplicationDbContext context)
        {
            _dBContext = context;
        }

        [HttpGet]
        public IActionResult FaultDashboard()
        {
            var tiles = new List<TileModel>
            {
                new TileModel {Title = "Report Fault", Description = "Report a fault for one of your fridges.", Action = "ReportFault", Controller = "Fault"},
                new TileModel {Title = "View Fault Status", Description = "View the status of your fridges currently in maintenace.", Action = "FaultDashboard", Controller = "Fault"},
                new TileModel {Title = "Request Replacement Fridge", Description = "Request a replacement fridge for your faulty fridges.", Action = "FaultDashboard", Controller = "Fault"}
            };

            return View(tiles);
        }

        // GET: Fault/Create
        public async Task<IActionResult> CreateFault()
        {
            // Get the logged-in UserID from the claims
            var userId = User.FindFirst("UserID")?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            // Find the Customer based on the UserID
            var customer = await _dBContext.Customer.FirstOrDefaultAsync(c => c.UserID == int.Parse(userId));
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            // Fetch the fridges allocated to this customer
            var allocatedFridges = await _dBContext.FridgeAllocation
                .Where(a => a.CustomerID == customer.CustomerID)
                .Select(a => new {
                    AllocationID = a.AllocationID,
                    FridgeName = $"{a.Fridge.FridgeModel}, {a.Fridge.SerialNumber}"
                }).ToListAsync();

            ViewBag.FridgeList = new SelectList(allocatedFridges, "AllocationID", "FridgeName");

            return View();
        }
    }
}
