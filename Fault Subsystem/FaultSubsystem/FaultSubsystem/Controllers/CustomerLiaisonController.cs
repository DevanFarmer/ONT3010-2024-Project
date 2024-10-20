using FaultSubsystem.Data;
using FaultSubsystem.Models.CustomerModels;
using FaultSubsystem.Models.CustomerLiaison;
using FaultSubsystem.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using FaultSubsystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using FaultSubsystem.Models.Account;

namespace FaultSubsystem.Controllers
{
    public class CustomerLiaisonController : Controller
    {
        private readonly ApplicationDbContext _dBContext;

        public CustomerLiaisonController(ApplicationDbContext context)
        {
            _dBContext = context;
        }

        public IActionResult Dashboard()
        {
            var tiles = new List<TileModel> 
            {
                new TileModel {Title = "Manage Customers", Description = "Manage customer information and allocations.", Action = "ViewCustomers", Controller = "CustomerLiaison"},
                new TileModel {Title = "Create Customer", Description = "Add a new customer to the system.", Action = "CreateCustomer", Controller = "CustomerLiaison"}
            };

            TempData["TilesList"] = JsonConvert.SerializeObject(tiles);

            return RedirectToAction("Dashboard", "Shared");
        }

        #region Manage Customer
        public async Task<IActionResult> ViewCustomers(string sortOrder, string searchString)
        {
            // Sorting logic
            ViewBag.FirstNameSortParm = string.IsNullOrEmpty(sortOrder) ? "firstName_desc" : "";
            ViewBag.LastNameSortParm = sortOrder == "LastName" ? "lastName_desc" : "LastName";
            var customers = from c in _dBContext.Customer
                            join u in _dBContext.User on c.UserID equals u.UserID
                            select new CustomerViewModel
                            {
                                CustomerID = c.CustomerID,
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                Email = u.Email,
                                PhoneNumber = u.PhoneNumber
                            };

            // Search logic
            if (!string.IsNullOrEmpty(searchString))
            {
                customers = customers.Where(c => c.FirstName.Contains(searchString)
                                               || c.LastName.Contains(searchString));
            }

            // Sorting the list
            switch (sortOrder)
            {
                case "firstName_desc":
                    customers = customers.OrderByDescending(c => c.FirstName);
                    break;
                case "LastName":
                    customers = customers.OrderBy(c => c.LastName);
                    break;
                case "lastName_desc":
                    customers = customers.OrderByDescending(c => c.LastName);
                    break;
                default:
                    customers = customers.OrderBy(c => c.FirstName);
                    break;
            }

            return View(await customers.ToListAsync());
        }

        public IActionResult CreateCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CreateAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Does email already exist
            var existingUser = await _dBContext.User.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                return View(model);
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Increment ID, when deleting is enabled you go through all ids and count, if no id of that exist that is the new id
            var maxUsers = await _dBContext.User.MaxAsync(u => (int?)u.UserID) ?? 0;

            var newUserID = maxUsers + 1;

            // Create the new user
            var user = new User
            {
                UserID = newUserID,
                Email = model.Email,
                Password = passwordHash,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                AccountActive = true
            };

            // Increment ID, when deleting is enabled you go through all ids and count, if no id of that exist that is the new id
            var maxCustomers = await _dBContext.Customer.MaxAsync(e => (int?)e.CustomerID) ?? 0;

            var newCustomerID = maxCustomers + 1;

            // Create the new employee
            var newCustomer = new Customer
            {
                CustomerID = newCustomerID,
                UserID = newUserID
            };

            try
            {
                _dBContext.User.Add(user);
            }
            catch
            {
                return View(model);
            }
            _dBContext.Customer.Add(newCustomer);
            await _dBContext.SaveChangesAsync();

            return RedirectToAction("Dashboard", "CustomerLiaison");
        }

        public async Task<IActionResult> EditCustomer(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _dBContext.Customer
                                        .Include(c => c.User)
                                        .FirstOrDefaultAsync(c => c.CustomerID == id);
            if (customer == null)
            {
                return NotFound();
            }

            var viewModel = new CustomerViewModel
            {
                CustomerID = customer.CustomerID,
                FirstName = customer.User.FirstName,
                LastName = customer.User.LastName,
                Email = customer.User.Email,
                PhoneNumber = customer.User.PhoneNumber
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditCustomer(int id, CustomerViewModel viewModel)
        {
            if (id != viewModel.CustomerID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var customer = await _dBContext.Customer.Include(c => c.User)
                                    .FirstOrDefaultAsync(c => c.CustomerID == id);
                if (customer == null)
                {
                    return NotFound();
                }

                // Update the User information
                customer.User.FirstName = viewModel.FirstName;
                customer.User.LastName = viewModel.LastName;
                customer.User.Email = viewModel.Email;
                customer.User.PhoneNumber = viewModel.PhoneNumber;

                await _dBContext.SaveChangesAsync();
                return RedirectToAction(nameof(ViewCustomers));
            }

            return View(viewModel);
        }
        #endregion

        #region Manage Allocations
        public async Task<IActionResult> ViewCustomerAllocations(int customerId)
        {
            // Log the incoming customer ID for debugging purposes
            Console.WriteLine($"Customer ID: {customerId}");

            // Fetch the customer by ID
            var customer = await _dBContext.Customer
                .Include(c => c.User) // Include the User information to avoid null reference later
                .FirstOrDefaultAsync(c => c.CustomerID == customerId);

            // Check if the customer exists
            if (customer == null)
            {
                return NotFound("Customer Not Found.");
            }

            // Fetch the allocations for the customer
            var allocations = await _dBContext.FridgeAllocation
                .Include(fa => fa.Fridge)
                    .ThenInclude(f => f.Inventory)
                .Where(fa => fa.CustomerID == customer.CustomerID)
                .ToListAsync();

            // Prepare the view model
            var model = new CustomerAllocationsViewModel
            {
                CustomerID = customer.CustomerID,
                FirstName = customer.User?.FirstName ?? "Unknown", // Default value if User is null
                LastName = customer.User?.LastName ?? "Unknown",   // Default value if User is null
                Allocations = allocations.Select(a => new AllocationViewModel
                {
                    FridgeModel = a.Fridge?.Inventory?.FridgeModel ?? "Fridge Not Found", // Use null-conditional operator
                    SerialNumber = a.Fridge?.SerialNumber ?? "N/A", // Use null-conditional operator
                    AllocationDate = a.AllocationDate.ToShortDateString(),
                    ReturnDate = a.ReturnDate.HasValue ? a.ReturnDate.Value.ToShortDateString() : "N/A"
                }).ToList()
            };

            return View(model);
        }

        public IActionResult AddFridgeAllocation(int customerId)
        {
            var model = new AddFridgeAllocationViewModel
            {
                CustomerID = customerId,
                AvailableFridges = _dBContext.Fridge
                    .Where(f => !f.FridgeAllocation.Any(fa => fa.ReturnDate == null))
                    .Select(f => new SelectListItem
                    {
                        Value = f.FridgeID.ToString(),
                        Text = _dBContext.Inventory
                                .Where(i => i.FridgeTypeID == f.FridgeTypeID)
                                .Select(i => i.FridgeModel)
                                .FirstOrDefault() + " - " + f.SerialNumber
                    }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddFridgeAllocation(AddFridgeAllocationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var allocation = new FridgeAllocation
                {
                    FridgeID = model.SelectedFridgeID,
                    CustomerID = model.CustomerID,
                    AllocationDate = DateTime.Now
                };

                _dBContext.FridgeAllocation.Add(allocation);
                await _dBContext.SaveChangesAsync();

                return RedirectToAction("ViewCustomerAllocations", new { customerId = model.CustomerID });
            }

            return View(model);
        }
        #endregion
    }
}
