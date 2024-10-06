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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewRole(Role model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(ViewRoles));
            }

            // Does description already exist
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(int id, [Bind("RoleID,RoleName")] Role model)
        {
            if (id != model.RoleID)
            {
                return NotFound(); // Returns 404, Show error message then return to ViewRoles
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dBContext.Update(model);
                    await _dBContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(model.RoleID))
                    {
                        return NotFound(); // returns 404
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return RedirectToAction(nameof(ViewRoles));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInLine(int id, [FromBody] Role updatedRole)
        {
            if (id != updatedRole.RoleID)
            {
                return BadRequest();
            }

            var role = await _dBContext.Role.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                role.RoleName = updatedRole.RoleName;
                try
                {
                    await _dBContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(role.RoleID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return Json(new {success = true, roleName = role.RoleName});
            }

            return Json(new { success = false, message = "Invalid data!"});
        }

        private bool RoleExists(int id)
        {
            return _dBContext.Role.Any(r => r.RoleID == id);
        }
    }
}
