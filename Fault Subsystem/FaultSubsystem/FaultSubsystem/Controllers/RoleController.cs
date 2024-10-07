using Microsoft.AspNetCore.Mvc;
using FaultSubsystem.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using FaultSubsystem.Data;
using FaultSubsystem.Models;
using Microsoft.AspNetCore.Identity;

namespace FaultSubsystem.Controllers
{
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _dBContext;

        public RoleController(ApplicationDbContext context)
        {
            _dBContext = context;
        }

        [HttpGet]
        public async Task<IActionResult> ViewRoles()
        {
            var roles = await _dBContext.Role.ToListAsync();
            return View(roles);
        }

        public IActionResult CreateNewRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewRole(Role model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(ViewRoles));
            }

            // Does role name already exist
            var existingRole = await _dBContext.Role.FirstOrDefaultAsync(r => r.RoleName == model.RoleName);

            if (existingRole != null)
            {
                ModelState.AddModelError("", "This role already exists.");
                return View(model);
            }

            // Increment ID
            var maxRoles = await _dBContext.Role.MaxAsync(r => (int?)r.RoleID) ?? 0;

            var newRoleID = maxRoles + 1;

            // Create the new role
            var role = new Role
            {
                RoleID = newRoleID,
                RoleName = model.RoleName
            };

            _dBContext.Role.Add(role);
            await _dBContext.SaveChangesAsync();

            return RedirectToAction(nameof(ViewRoles));
        }

        // GET: Role/EditRole/{id}
        public async Task<IActionResult> EditRole(int id)
        {
            var roles = await _dBContext.Role.ToListAsync();
            ViewData["EditRoleId"] = id;  // Pass the ID of the role being edited
            return View("ViewRoles", roles);   // Re-render the ViewRoles view with edit mode active for this role
        }

        // POST: Role/UpdateRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(int RoleID, string RoleName)
        {
            var role = await _dBContext.Role.FindAsync(RoleID);
            if (role == null)
            {
                return NotFound();
            }

            // Update the role name
            role.RoleName = RoleName;

            if (ModelState.IsValid)
            {
                await _dBContext.SaveChangesAsync();
                return RedirectToAction(nameof(ViewRoles));
            }

            // If invalid, return back to ViewRoles with edit mode still enabled
            var roles = await _dBContext.Role.ToListAsync();
            ViewData["EditRoleId"] = RoleID; // Keep the edit mode active if validation fails
            return View("ViewRoles", roles);
        }

        private bool RoleExists(int id)
        {
            return _dBContext.Role.Any(r => r.RoleID == id);
        }
    }
}
