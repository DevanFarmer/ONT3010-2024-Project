using FaultSubsystem.Data;
using FaultSubsystem.Models.CustomerModels;
using FaultSubsystem.Models.EmployeeModels;
using FaultSubsystem.Models.Fault;
using FaultSubsystem.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace FaultSubsystem.Controllers
{
    public class FaultTechnicianController : Controller
    {
        private readonly ApplicationDbContext _dBContext;

        public FaultTechnicianController(ApplicationDbContext context)
        {
            _dBContext = context;
        }

        public IActionResult Dashboard()
        {
            var tiles = new List<TileModel>
            {
                new TileModel {Title = "View Your Assigned Faults", Description = "View unassigned reports of faulty fridges.", Action = "ViewAssignedFaultReports", Controller = "FaultTechnician"},
                new TileModel {Title = "View Unassigned Faults", Description = "View unassigned reports of faulty fridges.", Action = "ViewUnassignedFaultyFridges", Controller = "FaultTechnician"},
                new TileModel {Title = "View Fault History", Description = "View previous faults you worked on.", Action = "ViewFaultReportsHistory", Controller = "FaultTechnician"}
            };

            TempData["TilesList"] = JsonConvert.SerializeObject(tiles);

            return RedirectToAction("Dashboard", "Shared");
        }

        public async Task<IActionResult> ViewUnassignedFaultyFridges()
        {
            var unassignedFaultReports = await _dBContext.FaultReport
                .Include(fr => fr.FaultStatus)
                .Include(fr => fr.FridgeAllocation)
                    .ThenInclude(fa => fa.Fridge)
                .ThenInclude(f => f.Inventory)
                .Where(fr => fr.EmployeeID == null)
                .Select(fr => new UnassignedFaultReportViewModel
                {
                    FaultID = fr.FaultID,
                    FaultDescription = fr.FaultDescription ?? "",
                    FaultStatus = fr.FaultStatus.StatusName,
                    FridgeModel = fr.FridgeAllocation.Fridge.Inventory.FridgeModel,
                    SerialNumber = fr.FridgeAllocation.Fridge.SerialNumber,
                    AllocationDate = fr.FridgeAllocation.AllocationDate.ToShortDateString()
                })
                .ToListAsync();

            return View(unassignedFaultReports);
        }

        [HttpPost]
        public async Task<IActionResult> AssignFaultReport(int faultID)
        {
            // Get employee's ID (should check if user is logged in)
            int employeeID = GetCurrentEmployeeID();

            // Find the fault report and assign the employee
            var faultReport = await _dBContext.FaultReport.FindAsync(faultID);
            if (faultReport == null)
            {
                return NotFound();
            }

            faultReport.EmployeeID = employeeID;
            faultReport.AssignedDate = DateTime.Now;
            await _dBContext.SaveChangesAsync();

            // Redirect back to the list of unassigned fault reports
            return RedirectToAction(nameof(ViewAssignedFaultReports));
        }

        public async Task<IActionResult> ViewAssignedFaultReports()
        {
            int employeeID = GetCurrentEmployeeID();

            // Check if found

            var faultReports = await _dBContext.FaultReport
                .Where(fr => fr.EmployeeID == employeeID && fr.FaultStatus.StatusName != "Resolved")
                .Select(fr => new FaultReportTechnicianViewModel
                {
                    FaultID = fr.FaultID,
                    FaultDescription = fr.FaultDescription ?? "",
                    ReportDate = fr.ReportDate.ToShortDateString(),
                    FaultStatus = fr.FaultStatus.StatusName,
                    AssignedDate = fr.AssignedDate.HasValue ? fr.AssignedDate.Value.ToShortDateString() : "N/A",
                    ScheduledRepairDate = fr.ScheduledRepairDate.HasValue ? fr.ScheduledRepairDate.Value.ToShortDateString() : "N/A",
                    Diagnosis = fr.Diagnosis ?? ""
                })
                .ToListAsync();

            return View(faultReports);
        }

        public async Task<IActionResult> EditFaultReport(int id)
        {
            var faultReport = await _dBContext.FaultReport
                .Include(fr => fr.FaultStatus) // Include status for display
                .FirstOrDefaultAsync(fr => fr.FaultID == id);

            if (faultReport == null)
            {
                return NotFound();
            }

            var faultReportViewModel = new EditFaultReportModel
            {
                FaultID = faultReport.FaultID,
                Diagnosis = faultReport.Diagnosis ?? "",
                FaultStatusID = faultReport.FaultStatusID,
                ScheduledRepairDate = faultReport.ScheduledRepairDate.HasValue ? faultReport.ScheduledRepairDate.Value : null,
                Notes = faultReport.Notes ?? "",
                AvailableStatuses = await _dBContext.FaultStatus.ToListAsync() // Dropdown for statuses
            };

            return View(faultReportViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditFaultReport(EditFaultReportModel model)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the fault report based on the FaultID
                var faultReport = await _dBContext.FaultReport
                    .Include(fr => fr.FridgeAllocation)
                        .ThenInclude(fa => fa.Fridge) // Include the fridge
                    .FirstOrDefaultAsync(fr => fr.FaultID == model.FaultID);

                if (faultReport != null)
                {
                    // Update the fault report details
                    faultReport.FaultStatusID = model.FaultStatusID;
                    faultReport.Notes = model.Notes;
                    faultReport.Diagnosis = model.Diagnosis;

                    // Check the fault status and update the fridge status
                    if (model.FaultStatusID == 3) // "Resolved"
                    {
                        faultReport.FridgeAllocation.Fridge.StatusID = 2; // "Allocated"
                    }
                    else if (model.FaultStatusID == 4) // "Should scrap"
                    {
                        faultReport.FridgeAllocation.Fridge.StatusID = 4; // "Scrapped"
                    }

                    // Save changes to the database
                    await _dBContext.SaveChangesAsync();

                    return RedirectToAction(nameof(ViewAssignedFaultReports));
                }

                return NotFound(); // Fault report not found
            }

            return View(model); // If model state is invalid, return to the same view
        }

        public async Task<IActionResult> ViewFaultReportsHistory()
        {
            int employeeID = GetCurrentEmployeeID();

            // Check if found

            var faultReports = await _dBContext.FaultReport
                .Where(fr => fr.EmployeeID == employeeID && fr.FaultStatus.StatusName == "Resolved")
                .Select(fr => new FaultReportTechnicianViewModel
                {
                    FaultID = fr.FaultID,
                    FaultDescription = fr.FaultDescription ?? "",
                    ReportDate = fr.ReportDate.ToShortDateString(),
                    FaultStatus = fr.FaultStatus.StatusName,
                    AssignedDate = fr.AssignedDate.HasValue ? fr.AssignedDate.Value.ToShortDateString() : "N/A",
                    ScheduledRepairDate = fr.ScheduledRepairDate.HasValue ? fr.ScheduledRepairDate.Value.ToShortDateString() : "N/A",
                    Diagnosis = fr.Diagnosis ?? ""
                })
                .ToListAsync();

            return View(faultReports);
        }

        public async Task<IActionResult> FaultReportDetails(int faultID)
        {
            var faultReport = await _dBContext.FaultReport
                .Include(fr => fr.FaultStatus)
                .Include(fr => fr.FridgeAllocation)
                    .ThenInclude(fa => fa.Fridge)
                        .ThenInclude(f => f.Inventory)
                .FirstOrDefaultAsync(fr => fr.FaultID == faultID);

            if (faultReport == null)
            {
                return NotFound();
            }

            var viewModel = new FaultReportDetailsViewModel
            {
                FaultID = faultReport.FaultID,
                FaultDescription = faultReport.FaultDescription ?? "",
                FaultStatus = faultReport.FaultStatus.StatusName,
                Diagnosis = faultReport.Diagnosis ?? "",
                Notes = faultReport.Notes ?? "",
                ScheduledRepairDate = faultReport.ScheduledRepairDate?.ToShortDateString() ?? "N/A",
                ReportDate = faultReport.ReportDate.ToShortDateString(),
                ResolutionDate = faultReport.ResolutionDate?.ToShortDateString() ?? "Not Resolved",
                FridgeModel = faultReport.FridgeAllocation.Fridge.Inventory.FridgeModel,
                SerialNumber = faultReport.FridgeAllocation.Fridge.SerialNumber,
                DateAcquired = faultReport.FridgeAllocation.Fridge.DateAcquired.ToShortDateString()
            };

            return View(viewModel);
        }

        private int GetCurrentEmployeeID()
        {
            var userID = User.FindFirstValue("UserID");
            var employee = _dBContext.Employee.FirstOrDefault(e => e.UserID == int.Parse(userID));
            return employee.EmployeeID;
        }
    }
}
