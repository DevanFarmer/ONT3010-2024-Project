using FaultSubsystem.Data;
using FaultSubsystem.Models.CustomerModels;
using FaultSubsystem.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
                new TileModel {Title = "Fridge Allocation", Description = "Allocate fridges to customers.", Action = "Dashboard", Controller = "CustomerLiaison"},
                new TileModel {Title = "View Customers", Description = "View all customers and their information.", Action = "Dashboard", Controller = "CustomerLiaison"}
            };

            TempData["TilesList"] = JsonConvert.SerializeObject(tiles);

            return RedirectToAction("Dashboard", "Shared");
        }

        public async Task<IActionResult> ViewCustomers(string sortOrder, string searchString)
        {
            // Sorting logic
            ViewBag.FirstNameSortParm = String.IsNullOrEmpty(sortOrder) ? "firstName_desc" : "";
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
            if (!String.IsNullOrEmpty(searchString))
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
    }
}
