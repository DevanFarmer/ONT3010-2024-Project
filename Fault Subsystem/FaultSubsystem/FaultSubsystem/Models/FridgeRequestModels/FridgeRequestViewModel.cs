using Microsoft.AspNetCore.Mvc.Rendering;

namespace FaultSubsystem.Models.FridgeRequestModels
{
    public class FridgeRequestViewModel
    {
        public int CustomerID { get; set; }
        public string SelectedFridgeModel { get; set; }
        public List<SelectListItem> AvailableFridges { get; set; }
    }
}
