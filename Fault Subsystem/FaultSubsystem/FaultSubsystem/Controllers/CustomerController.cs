using FaultSubsystem.Data;
using FaultSubsystem.Models;
using FaultSubsystem.Models.CustomerModels;
using FaultSubsystem.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FaultSubsystem.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _dBContext;

        public CustomerController(ApplicationDbContext context)
        {
            _dBContext = context;
        }

        public IActionResult Dashboard()
        {

            // Maybe just show the list of fridges they're allocated
            var tiles = new List<TileModel>
            {
                new TileModel {Title = "View Fridges", Description = "View your fridges.", Action = "ViewFridges", Controller = "Customer"},
                new TileModel {Title = "View Temporary Fridges", Description = "View your temporarily allocated fridges.", Action = "Dashboard", Controller = "Customer"}
            };

            TempData["TilesList"] = JsonConvert.SerializeObject(tiles);

            return RedirectToAction("Dashboard", "Shared");
        }

        public async Task<IActionResult> ViewFridges()
        {
            // get customer id then redirect to view
            Customer customer = await GetLoggedInCustomer();

            return RedirectToAction(nameof(ViewCustomerFridges), "Customer", new { customerID = customer.CustomerID });
        }

        public async Task<IActionResult> ViewCustomerFridges(int customerID)
        {
            var allocatedFridges = await (from allocation in _dBContext.FridgeAllocation
                                          join fridge in _dBContext.Fridge on allocation.FridgeID equals fridge.FridgeID
                                          join location in _dBContext.Location on fridge.LocationID equals location.LocationID
                                          where allocation.CustomerID == customerID
                                          select new AllocatedFridgesViewModel
                                          {
                                              FridgeID = fridge.FridgeID,
                                              FridgeModel = fridge.FridgeModel,
                                              SerialNumber = fridge.SerialNumber,
                                              Addressline1 = location.AddressLine1,
                                              Addressline2 = location.AddressLine2,
                                              City = location.City,
                                              PostalCode = location.PostalCode,
                                              AllocationID = allocation.AllocationID
                                          }).ToListAsync();

            return View(allocatedFridges);
        }

        public async Task<IActionResult> AllocatedFridgeDetails(int fridgeID)
        {
            // Fetch the fridge allocation using the provided ID
            var fridgeAllocation = await _dBContext.FridgeAllocation
                                    .Where(f => f.FridgeID == fridgeID)
                                    .Select(fa => new AllocatedFridgeDetailsViewModel
                                    {
                                        FridgeModel = fa.Fridge.FridgeModel,
                                        SerialNumber = fa.Fridge.SerialNumber,
                                        Addressline1 = fa.Fridge.Location.AddressLine1,
                                        Addressline2 = fa.Fridge.Location.AddressLine2,
                                        City = fa.Fridge.Location.City,
                                        PostalCode = fa.Fridge.Location.PostalCode,
                                        AllocationDate = fa.AllocationDate.ToShortDateString(),
                                        ReturnDate = fa.ReturnDate.HasValue ? fa.ReturnDate.Value.ToShortDateString() : "Not Returned"
                                    })
                                    .FirstOrDefaultAsync();

            if (fridgeAllocation == null)
            {
                return NotFound();
            }

            return View(fridgeAllocation);
        }

        public IActionResult ReportFault(int allocationID)
        {
            var model = new FaultReportViewModel
            {
                AllocationID = allocationID
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportFault(FaultReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                var faultReport = new FaultReport
                {
                    AllocationID = model.AllocationID,
                    FaultDescription = model.FaultDescription,
                    FaultStatusID = 1, // First Status should be 'Reported'
                    ReportDate = DateTime.Now
                };

                _dBContext.FaultReport.Add(faultReport);
                await _dBContext.SaveChangesAsync();

                return RedirectToAction(nameof(ViewFridges), "Customer");
            }

            return View(model); // Error messages
        }

        public async Task<IActionResult> CreateFault()
        {
            var customer = await GetLoggedInCustomer();

            if (customer == null)
            {
                // Show proper error
                return RedirectToAction("Dashboard", "Customer");
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

        public async Task<Customer> GetLoggedInCustomer()
        {
            // Get the logged-in UserID from the claims
            var userId = User.FindFirst("UserID")?.Value;

            if (userId == null)
            {
                // Show proper error
                return null;
            }

            // Find the Customer based on the UserID
            var customer = await _dBContext.Customer.FirstOrDefaultAsync(c => c.UserID == int.Parse(userId));
            if (customer == null)
            {
                // Show proper error
                return null;
            }

            return customer;
        }
    }
}
