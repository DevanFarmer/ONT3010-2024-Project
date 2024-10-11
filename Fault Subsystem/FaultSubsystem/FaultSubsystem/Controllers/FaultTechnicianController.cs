using FaultSubsystem.Data;
using FaultSubsystem.Models.EmployeeModels;
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
                new TileModel {Title = "Manage Faulty Fridges", Description = "Manage your assigned faulty fridges.", Action = "ReportFault", Controller = "FaultTechnician"},
                new TileModel {Title = "View All Faults", Description = "View unassigned reports of faulty fridges.", Action = "ViewUnassignedFaultyFridges", Controller = "FaultTechnician"}
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
                .Where(fr => fr.EmployeeID == null)
                .Select(fr => new UnassignedFaultReportViewModel
                {
                    FaultID = fr.FaultID,
                    FaultDescription = fr.FaultDescription,
                    FaultStatus = fr.FaultStatus.StatusName,
                    FridgeModel = fr.FridgeAllocation.Fridge.FridgeModel,
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
            var employeeID = GetCurrentEmployeeID();

            // Find the fault report and assign the employee
            var faultReport = await _dBContext.FaultReport.FindAsync(faultID);
            if (faultReport == null)
            {
                return NotFound();
            }

            faultReport.EmployeeID = employeeID;
            await _dBContext.SaveChangesAsync();

            // Redirect back to the list of unassigned fault reports
            return RedirectToAction(nameof(ViewUnassignedFaultyFridges));
        }

        private int GetCurrentEmployeeID()
        {
            var userID = User.FindFirstValue("UserID");
            var employee = _dBContext.Employee.FirstOrDefault(e => e.UserID == int.Parse(userID));
            return employee.EmployeeID;
        }
    }
}
