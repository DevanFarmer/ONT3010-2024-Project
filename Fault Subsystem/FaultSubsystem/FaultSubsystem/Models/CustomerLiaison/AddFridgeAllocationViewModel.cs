using Microsoft.AspNetCore.Mvc.Rendering;

namespace FaultSubsystem.Models.CustomerLiaison
{
    public class AddFridgeAllocationViewModel
    {
        public int CustomerID { get; set; }
        public int SelectedFridgeID { get; set; }

        public List<SelectListItem> AvailableFridges { get; set; }
    }
}
