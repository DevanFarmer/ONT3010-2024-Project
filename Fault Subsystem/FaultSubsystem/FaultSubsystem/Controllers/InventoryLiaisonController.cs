using FaultSubsystem.Data;
using FaultSubsystem.Models;
using FaultSubsystem.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

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
                new TileModel {Title = "Suppliers", Description = "View, Add and Edit supplier information.", Action = "Dashboard", Controller = "InventoryLiaison"},
                new TileModel {Title = "View Customers", Description = "View all customers and their information.", Action = "Dashboard", Controller = "InventoryLiaison"}
            };

            TempData["TilesList"] = JsonConvert.SerializeObject(tiles);

            return RedirectToAction("Dashboard", "Shared");
        }

        // View Suppliers
        public async Task<IActionResult> ViewSuppliers(string sortOrder, string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var suppliers = from s in _dBContext.Supplier select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                suppliers = suppliers.Where(s => s.SupplierName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    suppliers = suppliers.OrderByDescending(s => s.SupplierName);
                    break;
                default:
                    suppliers = suppliers.OrderBy(s => s.SupplierName);
                    break;
            }

            return View(await suppliers.ToListAsync());
        }

        // Add Supplier (GET)
        public IActionResult AddSupplier()
        {
            return View();
        }

        // Add Supplier (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSupplier(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _dBContext.Add(supplier);
                await _dBContext.SaveChangesAsync();
                return RedirectToAction(nameof(ViewSuppliers));
            }
            return View(supplier);
        }

        // Edit Supplier (GET)
        public async Task<IActionResult> EditSupplier(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _dBContext.Supplier.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // Edit Supplier (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSupplier(int id, Supplier supplier)
        {
            if (id != supplier.SupplierID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _dBContext.Update(supplier);
                await _dBContext.SaveChangesAsync();
                return RedirectToAction(nameof(ViewSuppliers));
            }
            return View(supplier);
        }
    }
}
