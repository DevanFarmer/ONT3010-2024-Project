using FaultSubsystem.Models.FridgeRequestModels;
using FaultSubsystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FaultSubsystem.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FaultSubsystem.Controllers
{
    public class FridgeRequestController : Controller
    {
        private readonly ApplicationDbContext _dBContext;

        public FridgeRequestController(ApplicationDbContext context)
        {
            _dBContext = context;
        }

        #region Customer
        public async Task<IActionResult> RequestFridge()
        {
            // Get the list of available fridge models from the Inventory table
            var availableFridgeModels = await _dBContext.Inventory
                .Select(i => new SelectListItem
                {
                    Value = i.FridgeModel,
                    Text = i.FridgeModel
                })
                .Distinct()
                .ToListAsync();

            // Create the view model
            var model = new RequestFridgeViewModel
            {
                AvailableFridgeModels = availableFridgeModels
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestFridge(RequestFridgeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var maxRequests = await _dBContext.FridgeRequest.MaxAsync(fr => (int?)fr.FridgeRequestID) ?? 0;

                var newRequestID = maxRequests + 1;

                var customerID = int.Parse(User.FindFirst("UserID")?.Value);
                Console.WriteLine(customerID);

                var fridgeRequest = new FridgeRequest
                {
                    FridgeRequestID = newRequestID,
                    CustomerID = customerID,
                    FridgeModel = model.SelectedFridgeModel,
                    Handled = false // Initially not handled
                };

                _dBContext.FridgeRequest.Add(fridgeRequest);
                await _dBContext.SaveChangesAsync();

                return RedirectToAction("Dashboard", "Customer"); // only customer uses this so handle accordingly
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

            return RedirectToAction(nameof(RequestFridge));
        }

        public async Task<IActionResult> ViewCustomerFridgeRequests()
        {
            // Get the logged-in customer's ID
            var customer = await _dBContext.Customer
                .FirstOrDefaultAsync(c => c.UserID == int.Parse(User.FindFirst("UserID").Value));

            if (customer == null)
            {
                return NotFound();  // Handle case where customer is not found
            }

            // Query to get all fridge requests for the customer that are not handled
            var fridgeRequests = await _dBContext.FridgeRequest
                .Where(fr => fr.CustomerID == int.Parse(User.FindFirst("UserID").Value) && !fr.Handled)  // Filter by customer and Handled == false
                .Select(fr => new ViewCustomerFridgeRequests
                {
                    FridgeRequestID = fr.FridgeRequestID,
                    FridgeModel = fr.FridgeModel,
                    AssignFridgeID = fr.AssignFridgeID ?? null,
                    Handled = fr.Handled
                })
                .ToListAsync();

            return View(fridgeRequests);
        }

        #endregion

        #region Inventory Liaison
        public async Task<IActionResult> ViewFridgeRequests()
        {
            var unhandledRequests = await _dBContext.FridgeRequest
                .Where(fr => !fr.Handled && !fr.AssignFridgeID.HasValue)  // Unhandled requests with no assigned fridge
                .Select(fr => new
                {
                    FridgeRequestID = fr.FridgeRequestID,
                    FridgeModel = fr.FridgeModel,  // Just use FridgeModel since FridgeTypeID isn't available here
                    Handled = fr.Handled,
                    AssignFridgeID = fr.AssignFridgeID
                })
                .ToListAsync();

            return View(unhandledRequests);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AssignFridge(int fridgeRequestID, int assignedFridgeID)
        //{
        //    var fridgeRequest = await _dBContext.FridgeRequest.FindAsync(fridgeRequestID);
        //    if (fridgeRequest != null)
        //    {
        //        fridgeRequest.AssignFridgeID = assignedFridgeID;
        //        fridgeRequest.Handled = true; // Mark the request as handled

        //        _dBContext.Update(fridgeRequest);
        //        await _dBContext.SaveChangesAsync();
        //    }

        //    return RedirectToAction(nameof(ViewFridgeRequests), "FridgeRequest"); // handled by inventory liaison so navigate to previous page
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignFridge(AssignFridgeViewModel model)
        {
            if (ModelState.IsValid && model.SelectedFridgeID.HasValue)
            {
                var fridgeRequest = await _dBContext.FridgeRequest.FindAsync(model.FridgeRequestID);

                if (fridgeRequest == null)
                {
                    return NotFound();
                }

                // Assign the fridge to the request
                fridgeRequest.AssignFridgeID = model.SelectedFridgeID.Value;
                fridgeRequest.Handled = true;

                // Update the status of the assigned fridge to 'Allocated'
                var assignedFridge = await _dBContext.Fridge.FindAsync(model.SelectedFridgeID.Value);
                if (assignedFridge != null)
                {
                    assignedFridge.StatusID = 2; // Allocated
                }

                await _dBContext.SaveChangesAsync();

                return RedirectToAction(nameof(ViewFridgeRequests));
            }

            // Re-populate the AvailableFridges in case of form resubmission
            model.AvailableFridges = await _dBContext.Fridge
                .Where(f => f.Inventory.FridgeModel == model.FridgeModel && f.StatusID == 1) // Filter by FridgeModel and Status 'Available'
                .Select(f => new SelectListItem
                {
                    Value = f.FridgeID.ToString(),
                    Text = f.Inventory.FridgeModel + " - " + f.SerialNumber
                })
                .ToListAsync();

            return View(model);
        }
        #endregion


        public async Task<IActionResult> ViewAssignedFridgeRequests()
        {
            var assignedRequests = await _dBContext.FridgeRequest
                .Include(fr => fr.FridgeModel)
                .Where(fr => fr.Handled && fr.AssignFridgeID.HasValue)
                .ToListAsync();

            return View(assignedRequests);
    }

    #region Customer Liaison
    public async Task<IActionResult> AssignFridgeToRequest(int fridgeRequestId)
        {
            // Get the fridge request by ID
            var fridgeRequest = await _dBContext.FridgeRequest
                .Include(fr => fr.Customer)
                .FirstOrDefaultAsync(fr => fr.FridgeRequestID == fridgeRequestId);

            if (fridgeRequest == null)
            {
                return NotFound();
            }

            // Create the view model to pass to the view
            var model = new AssignFridgeViewModel
            {
                FridgeRequestID = fridgeRequest.FridgeRequestID,
                FridgeModel = fridgeRequest.FridgeModel,
                AvailableFridges = await _dBContext.Fridge
                    .Where(f => f.Inventory.FridgeModel == fridgeRequest.FridgeModel && f.StatusID == 1) // "Available" status is 1
                    .Select(f => new SelectListItem
                    {
                        Value = f.FridgeID.ToString(),
                        Text = $"{f.Inventory.FridgeModel} - {f.SerialNumber}"
                    })
                    .ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AllocateFridge(int fridgeRequestID)
        {
            var fridgeRequest = await _dBContext.FridgeRequest.FindAsync(fridgeRequestID);
            if (fridgeRequest != null)
            {
                var allocation = new FridgeAllocation
                {
                    CustomerID = fridgeRequest.CustomerID,
                    FridgeID = fridgeRequest.AssignFridgeID.Value, // Use the assigned fridge
                    AllocationDate = DateTime.Now
                };

                _dBContext.FridgeAllocation.Add(allocation);
                fridgeRequest.Handled = true; // Mark as handled

                _dBContext.Update(fridgeRequest);
                await _dBContext.SaveChangesAsync();

                return RedirectToAction("ViewCustomerAllocations", "CustomerLiaison"); // handled by customer liaison
            }

            return View();
        }
        #endregion
    }
}
