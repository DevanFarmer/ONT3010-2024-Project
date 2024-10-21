using FaultSubsystem.Data;
using FaultSubsystem.Models;
using FaultSubsystem.Models.Shared;
using FaultSubsystem.Models.InventoryLiaison;
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
                new TileModel {Title = "Inventory", Description = "Manage Inventory.", Action = "ViewInventory", Controller = "InventoryLiaison"},
                new TileModel {Title = "Suppliers", Description = "Manage Suppliers.", Action = "ViewSuppliers", Controller = "InventoryLiaison"},
                new TileModel {Title = "Fridges", Description = "Manage Fridges.", Action = "ViewFridges", Controller = "InventoryLiaison"},
                new TileModel {Title = "Fridge Status", Description = "Manage Fridge Status.", Action = "ViewFridgeStatuses", Controller = "InventoryLiaison"}
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
        public async Task<IActionResult> ViewInventory()
        {
            var inventory = await _dBContext.Inventory.Include(i => i.Supplier).ToListAsync();
            return View(inventory);
        }

        public async Task<IActionResult> AddInventory()
        {
            var suppliers = await _dBContext.Supplier.ToListAsync();
            ViewBag.Suppliers = new SelectList(suppliers, "SupplierID", "SupplierName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddInventory(Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                var maxInventory = await _dBContext.Inventory.MaxAsync(v => (int?)v.FridgeTypeID) ?? 0;

                var newInventoryID = maxInventory + 1;

                inventory.FridgeTypeID = newInventoryID;

                _dBContext.Inventory.Add(inventory);
                await _dBContext.SaveChangesAsync();
                return RedirectToAction(nameof(ViewInventory));
            }

            return RedirectToAction(nameof(AddInventory));
        }

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

        [HttpPost]
        public async Task<IActionResult> EditInventory(int id, Inventory inventory)
        {
            if (id != inventory.FridgeTypeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _dBContext.Update(inventory);
                await _dBContext.SaveChangesAsync();
                return RedirectToAction(nameof(ViewInventory));
            }

            // Loop through the ModelState errors
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                foreach (var error in state.Errors)
                {
                    // Log the error message
                    Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                }
            }

            return RedirectToAction(nameof(EditInventory), id);
        }
        #endregion

        #region Manage Fridges
        public async Task<IActionResult> ViewFridges()
        {
            var fridges = await _dBContext.Fridge
                .Include(f => f.Inventory)
                .Include(f => f.Status)
                .Include(f => f.Location)
                .Select(f => new FridgeViewModel
                {
                    FridgeID = f.FridgeID,
                    FridgeModel = f.Inventory.FridgeModel ?? "N/A",
                    SerialNumber = f.SerialNumber ?? "N/A",
                    DateAcquired = f.DateAcquired.ToShortDateString() ?? "N/A",
                    StatusName = f.Status.StatusName ?? "N/A",
                    AddressLine1 = f.Location.AddressLine1 ?? "N/A",
                    City = f.Location.City ?? "N/A",
                    PostalCode = f.Location.PostalCode ?? "N/A"
                })
                .ToListAsync();

            return View(fridges);
        }

        public IActionResult AddFridge()
        {
            var viewModel = new AddFridgeViewModel
            {
                AvailableStatuses = new SelectList(_dBContext.Status.ToList(), "StatusID", "StatusName"),
                AvailableLocations = new SelectList(_dBContext.Location
                .Select(l => new
                {
                    l.LocationID,
                    FullAddress = l.AddressLine1 + " " +
                      (string.IsNullOrEmpty(l.AddressLine2) ? "" : l.AddressLine2 + ", ") +
                      l.City + ", " +
                      l.PostalCode
                }).ToList(), "LocationID", "FullAddress"),
                AvailableFridgeModels = new SelectList(_dBContext.Inventory.ToList(), "FridgeTypeID", "FridgeModel")
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddFridge(AddFridgeViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Increment ID, when deleting is enabled you go through all ids and count, if no id of that exist that is the new id
                var maxFridges = await _dBContext.Fridge.MaxAsync(f => (int?)f.FridgeID) ?? 0;

                var newFridgeID = maxFridges + 1;

                var fridge = new Fridge
                {
                    FridgeID = newFridgeID,
                    FridgeTypeID = model.FridgeTypeID,
                    SerialNumber = model.SerialNumber,
                    StatusID = 1,
                    LocationID = model.LocationID,
                    DateAcquired = model.DateAcquired
                };

                _dBContext.Fridge.Add(fridge);
                await _dBContext.SaveChangesAsync();
                return RedirectToAction(nameof(ViewFridges));
            }

            // Loop through the ModelState errors
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                foreach (var error in state.Errors)
                {
                    // Log the error message
                    Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                }
            }

            model.AvailableStatuses = new SelectList(_dBContext.Status.ToList(), "StatusID", "StatusName");
            model.AvailableLocations = new SelectList(_dBContext.Location
                .Select(l => new
                {
                    l.LocationID,
                    FullAddress = l.AddressLine1 + " " +
                      (string.IsNullOrEmpty(l.AddressLine2) ? "" : l.AddressLine2 + ", ") +
                      l.City + ", " +
                      l.PostalCode
                }).ToList(), "LocationID", "FullAddress");
            model.AvailableFridgeModels = new SelectList(_dBContext.Inventory.ToList(), "FridgeTypeID", "FridgeModel");
            return View(model);
        }

        public async Task<IActionResult> EditFridge(int id)
        {
            var fridge = await _dBContext.Fridge.FindAsync(id);
            if (fridge == null)
            {
                return NotFound();
            }

            var viewModel = new EditFridgeViewModel
            {
                FridgeID = fridge.FridgeID,
                FridgeTypeID = fridge.FridgeTypeID,
                SerialNumber = fridge.SerialNumber,
                FridgeStatusID = fridge.StatusID,
                LocationID = fridge.LocationID,
                DateAcquired = fridge.DateAcquired,

                AvailableStatuses = new SelectList(_dBContext.Status.ToList(), "FridgeStatusID", "StatusName"),
                AvailableLocations = new SelectList(_dBContext.Location
                .Select(l => new
                {
                    l.LocationID,
                    FullAddress = l.AddressLine1 + " " +
                      (string.IsNullOrEmpty(l.AddressLine2) ? "" : l.AddressLine2 + ", ") +
                      l.City + ", " +
                      l.PostalCode
                }).ToList(), "LocationID", "FullAddress"),
                AvailableFridgeModels = new SelectList(_dBContext.Inventory.ToList(), "FridgeTypeID", "FridgeModel")
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditFridge(EditFridgeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var fridge = await _dBContext.Fridge.FindAsync(model.FridgeID);
                if (fridge == null)
                {
                    return NotFound();
                }

                fridge.FridgeTypeID = model.FridgeTypeID;
                fridge.SerialNumber = model.SerialNumber;
                fridge.StatusID = model.FridgeStatusID;
                fridge.LocationID = model.LocationID;
                fridge.DateAcquired = model.DateAcquired;

                await _dBContext.SaveChangesAsync();
                return RedirectToAction(nameof(ViewFridges));
            }

            // Loop through the ModelState errors
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                foreach (var error in state.Errors)
                {
                    // Log the error message
                    Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                }
            }

            model.AvailableStatuses = new SelectList(_dBContext.Status.ToList(), "StatusID", "StatusName");
            model.AvailableLocations = new SelectList(_dBContext.Location
                .Select(l => new
                {
                    l.LocationID,
                    FullAddress = l.AddressLine1 + " " +
                      (string.IsNullOrEmpty(l.AddressLine2) ? "" : l.AddressLine2 + ", ") +
                      l.City + ", " +
                      l.PostalCode
                }).ToList(), "LocationID", "FullAddress");
            model.AvailableFridgeModels = new SelectList(_dBContext.Inventory.ToList(), "FridgeTypeID", "FridgeModel");
            return View(model);
        }
        #endregion

        #region Fridge Status
        public async Task<IActionResult> ViewFridgeStatuses()
        {
            var fridgeStatuses = await _dBContext.Status
                .Select(s => new FridgeStatusViewModel
                {
                    FridgeStatusID = s.FridgeStatusID,
                    StatusName = s.StatusName
                })
                .ToListAsync();

            return View(fridgeStatuses);
        }

        [HttpGet]
        public IActionResult AddFridgeStatus()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFridgeStatus(FridgeStatusViewModel model)
        {
            if (ModelState.IsValid)
            {
                var maxStatus = await _dBContext.Status.MaxAsync(s => (int?)s.FridgeStatusID) ?? 0;

                var newStatusID = maxStatus + 1;

                var newStatus = new Status
                {
                    FridgeStatusID = newStatusID,
                    StatusName = model.StatusName
                };

                _dBContext.Status.Add(newStatus);
                await _dBContext.SaveChangesAsync();
                return RedirectToAction(nameof(ViewFridgeStatuses));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditFridgeStatus(int id)
        {
            var status = await _dBContext.Status.FindAsync(id);

            if (status == null)
            {
                return NotFound();
            }

            var model = new FridgeStatusViewModel
            {
                FridgeStatusID = status.FridgeStatusID,
                StatusName = status.StatusName
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFridgeStatus(FridgeStatusViewModel model)
        {
            if (ModelState.IsValid)
            {
                var status = await _dBContext.Status.FindAsync(model.FridgeStatusID);

                Console.WriteLine($"Fridge Status ID: {model.FridgeStatusID}");

                if (status == null)
                {
                    return NotFound();
                }

                status.StatusName = model.StatusName;

                await _dBContext.SaveChangesAsync();
                return RedirectToAction(nameof(ViewFridgeStatuses));
            }

            return View(model);
        }
        #endregion
    }
}
