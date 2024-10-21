using FaultSubsystem.Data;
using FaultSubsystem.Models;
using FaultSubsystem.Models.CustomerModels;
using FaultSubsystem.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
            //var allocatedFridges = await (from allocation in _dBContext.FridgeAllocation
            //                              join fridge in _dBContext.Fridge on allocation.FridgeID equals fridge.FridgeID
            //                              join inventory in _dBContext.Inventory on fridge.FridgeTypeID equals inventory.FridgeTypeID
            //                              join location in _dBContext.Location on fridge.LocationID equals location.LocationID
            //                              join faultReport in _dBContext.FaultReport on allocation.AllocationID equals faultReport.AllocationID into faultGroup
            //                              from fg in faultGroup.DefaultIfEmpty()  // To include fridges even if there are no faults
            //                              where allocation.CustomerID == customerID
            //                              && (fg == null || fg.FaultStatus.StatusName == "Resolved")  // Exclude unresolved faults
            //                              select new AllocatedFridgesViewModel
            //                              {
            //                                  FridgeID = fridge.FridgeID,
            //                                  FridgeModel = inventory.FridgeModel,
            //                                  SerialNumber = fridge.SerialNumber,
            //                                  Addressline1 = location.AddressLine1,
            //                                  Addressline2 = location.AddressLine2,
            //                                  City = location.City,
            //                                  PostalCode = location.PostalCode,
            //                                  AllocationID = allocation.AllocationID
            //                              }).ToListAsync();

            //return View(allocatedFridges);
            var allocatedFridges = await (from allocation in _dBContext.FridgeAllocation
                                          join fridge in _dBContext.Fridge on allocation.FridgeID equals fridge.FridgeID
                                          join inventory in _dBContext.Inventory on fridge.FridgeTypeID equals inventory.FridgeTypeID
                                          join location in _dBContext.Location on fridge.LocationID equals location.LocationID
                                          join fridgeStatus in _dBContext.Status on fridge.StatusID equals fridgeStatus.FridgeStatusID
                                          where allocation.CustomerID == customerID
                                          && fridgeStatus.StatusName == "Allocated"
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

                // Update FridgeStatusID to "Faulty"
                var fridgeAllocation = await _dBContext.FridgeAllocation
                    .Include(fa => fa.Fridge) // Include fridge data
                    .FirstOrDefaultAsync(fa => fa.AllocationID == model.AllocationID);

                if (fridgeAllocation != null)
                {
                    fridgeAllocation.Fridge.StatusID = 3; // Update FridgeStatusID to "Faulty"
                }
                else
                {
                    ModelState.AddModelError("FaultDescription", "Could not find fridge. Please try again later.");
                    return View(model);
                }

                await _dBContext.SaveChangesAsync();

                return RedirectToAction(nameof(ViewFridges), "Customer");
            }

            return View(model);
        }

        public async Task<IActionResult> ViewFaultyFridges()
        {
            var customer = await _dBContext.Customer
                .FirstOrDefaultAsync(c => c.UserID == int.Parse(User.FindFirst("UserID").Value));

            if (customer == null)
            {
                return NotFound();
            }

            //var faultReports = await _dBContext.FridgeAllocation
            //    .Include(fa => fa.Fridge)                          // Include Fridge
            //        .ThenInclude(f => f.Inventory)                 // Include Inventory for FridgeModel
            //    .Include(fa => fa.FaultReport)                     // Include FaultReports
            //        .ThenInclude(fr => fr.FaultStatus)             // Include FaultStatus for each report
            //    .Where(fa => fa.CustomerID == customer.CustomerID) // Filter by the logged-in customer
            //    .SelectMany(fa => fa.FaultReport
            //        .Where(fr => fr.FaultStatus.StatusName != "Resolved") // Filter out resolved faults
            //        .Select(fr => new FaultyFridgesViewModel
            //        {
            //            FridgeID = fa.FridgeID,
            //            FridgeModel = fa.Fridge.Inventory.FridgeModel,
            //            SerialNumber = fa.Fridge.SerialNumber,
            //            FaultID = fr.FaultID,
            //            FaultDescription = fr.FaultDescription,
            //            FaultStatus = fr.FaultStatus.StatusName,
            //            ReturnDate = fa.ReturnDate.HasValue ? fa.ReturnDate.Value.ToShortDateString() : "N/A"
            //        }))
            //    .ToListAsync();

            //return View(faultReports);
            var faultReports = await _dBContext.FridgeAllocation
                .Include(fa => fa.Fridge)                          // Include Fridge
                    .ThenInclude(f => f.Inventory)                 // Include Inventory for FridgeModel
                .Where(fa => fa.CustomerID == customer.CustomerID)  // Filter by the logged-in customer
                .Where(fa => fa.Fridge.StatusID == 3)               // Filter by Fridge status (Faulty)
                .Select(fa => new FaultyFridgesViewModel
                {
                    FridgeID = fa.FridgeID,
                    FridgeModel = fa.Fridge.Inventory.FridgeModel,
                    SerialNumber = fa.Fridge.SerialNumber,
                    FaultID = fa.FaultReport.Select(fr => fr.FaultID).FirstOrDefault(),  // Get FaultID from FaultReport
                    FaultDescription = fa.FaultReport.Select(fr => fr.FaultDescription).FirstOrDefault() ?? "N/A",
                    FaultStatus = fa.FaultReport.Select(fr => fr.FaultStatus.StatusName).FirstOrDefault() ?? "N/A",
                    ReturnDate = fa.ReturnDate.HasValue ? fa.ReturnDate.Value.ToShortDateString() : "N/A"
                })
                .ToListAsync();

            return View(faultReports);
        }

        public async Task<IActionResult> ViewFaultyFridgeDetails(int faultID)
        {
            var faultDetails = await _dBContext.FaultReport
                .Include(fr => fr.FridgeAllocation.Fridge)
                    .ThenInclude(f => f.Inventory)
                .Include(fr => fr.FridgeAllocation.Fridge.Location)
                .Include(fr => fr.FaultStatus)
                .Where(fr => fr.FaultID == faultID)
                .Select(fr => new FaultyFridgeDetailsViewModel
                {
                    FridgeID = fr.FridgeAllocation.FridgeID,
                    FridgeModel = fr.FridgeAllocation.Fridge.Inventory.FridgeModel,
                    SerialNumber = fr.FridgeAllocation.Fridge.SerialNumber,
                    Addressline1 = fr.FridgeAllocation.Fridge.Location.AddressLine1,
                    Addressline2 = fr.FridgeAllocation.Fridge.Location.AddressLine2,
                    City = fr.FridgeAllocation.Fridge.Location.City,
                    PostalCode = fr.FridgeAllocation.Fridge.Location.PostalCode,
                    AllocationDate = fr.FridgeAllocation.AllocationDate.ToShortDateString() ?? "N/A",
                    ReturnDate = fr.FridgeAllocation.ReturnDate.HasValue ? fr.FridgeAllocation.ReturnDate.Value.ToShortTimeString() : "N/A",
                    FaultReport = new FaultyFridgeFaultReportViewModel
                    {
                        FaultDescription = fr.FaultDescription ?? "N/A",
                        FaultStatus = fr.FaultStatus.StatusName ?? "N/A",
                        Diagnosis = fr.Diagnosis ?? "N/A",
                        ScheduledRepairDate = fr.ScheduledRepairDate.HasValue ? fr.ScheduledRepairDate.Value.ToShortDateString() : "N/A",
                        Notes = fr.Notes ?? "N/A"
                    }
                })
                .FirstOrDefaultAsync();

            if (faultDetails == null)
            {
                return NotFound();
            }

            return View(faultDetails);
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
