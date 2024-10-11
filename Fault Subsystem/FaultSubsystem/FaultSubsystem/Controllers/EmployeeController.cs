using FaultSubsystem.Data;
using FaultSubsystem.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using FaultSubsystem.Models.EmployeeModels;
using FaultSubsystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Specialized;
using FaultSubsystem.Models.CustomerModels;

namespace FaultSubsystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _dBContext;

        public EmployeeController(ApplicationDbContext dbContext)
        {
            _dBContext = dbContext;
        }

        public IActionResult Dashboard()
        {
            var tiles = new List<TileModel>
            {
                new TileModel {Title = "View Employees", Description = "View employee details.", Action = "ViewEmployees", Controller = "Employee"},
                new TileModel {Title = "Create Employees", Description = "Create new employees from users.", Action = "ViewNonEmployeeUsers", Controller = "Employee"}
            };

            TempData["TilesList"] = JsonConvert.SerializeObject(tiles);

            return RedirectToAction("Dashboard", "Shared");
        }

        public async Task<IActionResult> ViewEmployees()
        {
            var employees = await (from e in _dBContext.Employee
                                   join u in _dBContext.User on e.UserID equals u.UserID
                                   join r in _dBContext.Role on e.RoleID equals r.RoleID into er
                                   from role in er.DefaultIfEmpty()
                                   select new EmployeeViewModel
                                   {
                                       EmployeeID = e.EmployeeID,
                                       FullName = u.FirstName + " " + u.LastName,
                                       RoleName = role != null ? role.RoleName : "No Role Assigned"
                                   }).ToListAsync();

            return View(employees);
        }

        public async Task<IActionResult> EmployeeDetails(int id)
        {
            var employee = await (from e in _dBContext.Employee
                                  join u in _dBContext.User on e.UserID equals u.UserID
                                  join r in _dBContext.Role on e.RoleID equals r.RoleID into er
                                  from role in er.DefaultIfEmpty()
                                  where e.EmployeeID == id
                                  select new EmployeeViewDetailsModel
                                  {
                                      EmployeeID = e.EmployeeID,
                                      FirstName = u.FirstName,
                                      LastName = u.LastName,
                                      Email = u.Email,
                                      PhoneNumber = u.PhoneNumber,
                                      RoleName = role != null ? role.RoleName : "No Role Assigned",
                                      RoleID = role.RoleID
                                  }).FirstOrDefaultAsync();

            if (employee == null)
            {
                return NotFound();
            }

            ViewBag.RoleList = new SelectList(await _dBContext.Role.ToListAsync(), "RoleID", "RoleName", employee.RoleID);

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(int employeeID, int roleID)
        {
            var employee = await _dBContext.Employee.FindAsync(employeeID);
            if (employee == null)
            {
                return NotFound();
            }

            employee.RoleID = roleID;
            await _dBContext.SaveChangesAsync();

            return RedirectToAction(nameof(ViewEmployees));
        }

        public async Task<IActionResult> ViewNonEmployeeUsers(string searchString)
        {
            var usersQuery = from u in _dBContext.User
                             where !(from e in _dBContext.Employee select e.UserID).Contains(u.UserID)
                             select u;

            // search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                usersQuery = usersQuery.Where(u => u.FirstName.Contains(searchString) || u.LastName.Contains(searchString));
            }

            var users = await usersQuery.ToListAsync();

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> CreateEmployee(int userID)
        {
            // check if user already exists, just in case
            var existinEmployee = await _dBContext.Employee.FirstOrDefaultAsync(e => e.UserID == userID);
            if (existinEmployee != null)
            {
                return BadRequest("This user is already an employee");
            }

            // Increment ID, when deleting is enabled you go through all ids and count, if no id of that exist that is the new id
            var maxEmployees = await _dBContext.Employee.MaxAsync(e => (int?)e.EmployeeID) ?? 0;

            var newEmployeeID = maxEmployees + 1;

            // Create the new employee
            var newEmployee = new Employee
            {
                EmployeeID = newEmployeeID,
                UserID = userID,
                RoleID = 1
            };

            _dBContext.Employee.Add(newEmployee);
            await _dBContext.SaveChangesAsync();

            return RedirectToAction(nameof(ViewNonEmployeeUsers));
        }
    }
}
