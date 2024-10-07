using FaultSubsystem.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FaultSubsystem.Controllers
{
    public class SharedController : Controller
    {
        public IActionResult Dashboard()
        {
            // all redirects to this action should create a TempData for the tiles
            var tilesList = TempData["TilesList"] != null
                ? JsonConvert.DeserializeObject<List<TileModel>>(TempData["TilesList"].ToString())
                : new List<TileModel>();

            return View(tilesList);
        }
    }
}
