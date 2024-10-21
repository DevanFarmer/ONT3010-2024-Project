using Microsoft.AspNetCore.Mvc.Rendering;

namespace FaultSubsystem.Models.FridgeRequestModels
{
    public class AssignFridgeViewModel
    {
        public int FridgeRequestID { get; set; }
        public string FridgeModel { get; set; }
        public IEnumerable<SelectListItem> AvailableFridges { get; set; }
        public int? SelectedFridgeID { get; set; }
    }
}
