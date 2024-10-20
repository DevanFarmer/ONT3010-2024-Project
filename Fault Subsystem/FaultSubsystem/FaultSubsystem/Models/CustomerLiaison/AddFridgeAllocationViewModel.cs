using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FaultSubsystem.Models.CustomerLiaison
{
    public class AddFridgeAllocationViewModel
    {
        public int CustomerID { get; set; }
        [Required(ErrorMessage = "Please select a fridge.")]
        public int SelectedFridgeID { get; set; }

        public List<SelectListItem> AvailableFridges { get; set; }
    }
}
