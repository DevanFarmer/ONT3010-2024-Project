using FaultSubsystem.Data;
using FaultSubsystem.Models;
using FaultSubsystem.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

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
                new TileModel {Title = "Inventory", Description = "View , Add and Edit Inventory.", Action = "ViewInventory", Controller = "InventoryLiaison"},
                new TileModel {Title = "Suppliers", Description = "View, Add and Edit supplier information.", Action = "ViewSuppliers", Controller = "InventoryLiaison"}
            };

            TempData["TilesList"] = JsonConvert.SerializeObject(tiles);

            return RedirectToAction("Dashboard", "Shared");
        }

        #region Suppliers
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
            var maxSuppliers = await _dBContext.Supplier.MaxAsync(s => (int?)s.SupplierID) ?? 0;

            var newSupplierID = maxSuppliers + 1;

            supplier.SupplierID = newSupplierID;

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
        #endregion

        #region Inventory
        // View Inventory
        public async Task<IActionResult> ViewInventory()
        {
            var inventory = await _dBContext.Inventory.Include(i => i.Supplier).ToListAsync();
            return View(inventory);
        }

        // Get Add Inventory
        public async Task<IActionResult> AddInventory()
        {
            var suppliers = await _dBContext.Supplier.ToListAsync();
            ViewBag.Suppliers = new SelectList(suppliers, "SupplierID", "SupplierName");
            return View();
        }

        // Post Add Inventory
        [HttpPost]
        public async Task<IActionResult> AddInventory(Inventory inventory)
        {
            var maxInventory = await _dBContext.Inventory.MaxAsync(v => (int?)v.FridgeTypeID) ?? 0;

            var newInventoryID = maxInventory + 1;

            inventory.FridgeTypeID = newInventoryID;

            _dBContext.Inventory.Add(inventory);
            await _dBContext.SaveChangesAsync();
            return RedirectToAction(nameof(ViewInventory));
        }

        // Get Edit Inventory
        public async Task<IActionResult> EditInventory(int id)
        {
            var inventory = await _dBContext.Inventory.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }

            var suppliers = await _dBContext.Supplier.ToListAsync();
            ViewBag.Suppliers = new SelectList(suppliers, "SupplierID", "SupplierName", inventory.SupplierID);
            return View(inventory);
        }

        // Post Edit Inventory
        [HttpPost]
        public async Task<IActionResult> EditInventory(int id, Inventory inventory)
        {
            if (id != inventory.FridgeTypeID)
            {
                return NotFound();
            }

            _dBContext.Update(inventory);
            await _dBContext.SaveChangesAsync();
            return RedirectToAction(nameof(ViewInventory));
        }
        #endregion
    }
}
