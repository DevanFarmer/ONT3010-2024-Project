using Microsoft.AspNetCore.Mvc;

namespace FaultSubsystem.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Dashboard()
        {
            return RedirectToAction("Dashboard", "Shared");
        }
    }
}
