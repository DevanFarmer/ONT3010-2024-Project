using FaultSubsystem.Data;
using FaultSubsystem.Models;
using FaultSubsystem.Models.CustomerModels;
using FaultSubsystem.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

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
                new TileModel {Title = "View Temporary Fridges", Description = "View your temporarily allocated fridges.", Action = "Dashboard", Controller = "Customer"},
                new TileModel {Title = "View Faulty Fridges", Description = "View the status of your faulty fridges.", Action = "ViewFaultyFridges", Controller = "Customer"}
            };

            TempData["TilesList"] = JsonConvert.SerializeObject(tiles);

            return RedirectToAction("Dashboard", "Shared");
        }

        #region Fridge Information
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
                                          join inventory in _dBContext.Inventory on fridge.FridgeTypeID equals inventory.FridgeTypeID
                                          join location in _dBContext.Location on fridge.LocationID equals location.LocationID
                                          where allocation.CustomerID == customerID
                                          select new AllocatedFridgesViewModel
                                          {
                                              FridgeID = fridge.FridgeID,
                                              FridgeModel = inventory.FridgeModel,
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
                                        FridgeModel = fa.Fridge.Inventory.FridgeModel,
                                        SerialNumber = fa.Fridge.SerialNumber,
                                        Addressline1 = fa.Fridge.Location.AddressLine1,
                                        Addressline2 = fa.Fridge.Location.AddressLine2,
                                        City = fa.Fridge.Location.City,
                                        PostalCode = fa.Fridge.Location.PostalCode,
                                        AllocationDate = fa.AllocationDate.ToShortDateString(),
                                        ReturnDate = fa.ReturnDate.HasValue ? fa.ReturnDate.Value.ToShortDateString() : "N/A"
                                    })
                                    .FirstOrDefaultAsync();

            if (fridgeAllocation == null)
            {
                return NotFound();
            }

            return View(fridgeAllocation);
        }
        #endregion

        #region Faults
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
                // Increment ID, no deleting reports for records sake
                var maxReports = await _dBContext.FaultReport.MaxAsync(fr => (int?)fr.FaultID) ?? 0;

                var newFaultID = maxReports + 1;

                var faultReport = new FaultReport
                {
                    FaultID = newFaultID,
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

        public async Task<IActionResult> ViewFaultyFridges()
        {
            var customer = await _dBContext.Customer.FirstOrDefaultAsync(c => c.UserID == int.Parse(User.FindFirst("UserID").Value));

            if (customer == null)
            {
                return NotFound();
            }

            // Check Report Records for Status, If not 'Fixed' then it is still faulty
            var faultyFridges = await _dBContext.FridgeAllocation
                .Include(fa => fa.Fridge)
                    .ThenInclude(f => f.Inventory)
                .Include(fa => fa.FaultReport)
                    .ThenInclude(fr => fr.FaultStatus)
                .Where(fa => fa.CustomerID == customer.CustomerID) // Filter by customer
                .Where(fa => fa.FaultReport.Any(fr => fr.FaultStatus.StatusName != "Resolved")) // Filter by reports that are not fixed
                .Select(fa => new FaultyFridgesViewModel
                {
                    FridgeID = fa.FridgeID,
                    FridgeModel = fa.Fridge.Inventory.FridgeModel,
                    SerialNumber = fa.Fridge.SerialNumber,
                    FaultDescription = fa.FaultReport.First(fr => fr.FaultStatus.StatusName != "Resolved").FaultDescription,
                    FaultStatus = fa.FaultReport.First(fr => fr.FaultStatus.StatusName != "Resolved").FaultStatus.StatusName,
                    ReturnDate = fa.ReturnDate.HasValue ? fa.ReturnDate.Value.ToShortDateString() : "N/A"
                })
                .ToListAsync();

            return View(faultyFridges);
        }

        public async Task<IActionResult> ViewFaultyFridgeDetails(int fridgeID)
        {
            var faultyFridgeDetails = await _dBContext.FridgeAllocation
                .Include(fa => fa.Fridge)
                    .ThenInclude(f => f.Location)
                .Include(fa => fa.FaultReport)
                    .ThenInclude(fr => fr.FaultStatus) // Include fault status
                .Where(fa => fa.FridgeID == fridgeID) // Filter by FridgeID
                .Select(fa => new FaultyFridgeDetailsViewModel
                {
                    // Get Fridge Model
                    SerialNumber = fa.Fridge.SerialNumber,
                    Addressline1 = fa.Fridge.Location.AddressLine1,
                    Addressline2 = fa.Fridge.Location.AddressLine2,
                    City = fa.Fridge.Location.City,
                    PostalCode = fa.Fridge.Location.PostalCode,
                    AllocationDate = fa.AllocationDate.ToShortDateString(),
                    ReturnDate = fa.ReturnDate.HasValue ? fa.ReturnDate.Value.ToShortDateString() : "Not Returned",

                    // Selecting the first fault report that isn't "Fixed" for the fridge allocation
                    FaultReport = fa.FaultReport
                        .Where(fr => fr.FaultStatus.StatusName != "Resolved")
                        .Select(fr => new FaultyFridgeDetailsFaultReportViewModel
                        {
                            FaultDescription = fr.FaultDescription,
                            FaultStatus = fr.FaultStatus.StatusName
                        }).FirstOrDefault() ?? new FaultyFridgeDetailsFaultReportViewModel
                        {
                            FaultDescription = "N/A",
                            FaultStatus = "N/A"
                        }
                })
                .FirstOrDefaultAsync();

            return View(faultyFridgeDetails);
        }
        #endregion

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
