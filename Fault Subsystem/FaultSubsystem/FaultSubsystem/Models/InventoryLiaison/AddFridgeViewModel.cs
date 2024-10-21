using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FaultSubsystem.Models.InventoryLiaison
{
    public class AddFridgeViewModel
    {
        [Required(ErrorMessage = "Please Select a Fridge Type")]
        public int FridgeTypeID { get; set; }

        [Required(ErrorMessage = "Serial Number is Required")]
        [StringLength(55, MinimumLength = 3, ErrorMessage = "First Name must be between 3 and 55 characters")]
        public string SerialNumber { get; set; }

        [Required(ErrorMessage = "Please Select a Status")]
        public int FridgeStatusID { get; set; }

        [Required(ErrorMessage = "Please Select a Location")]
        public int LocationID { get; set; }

        public DateTime DateAcquired { get; set; }

        public SelectList? AvailableStatuses { get; set; }
        public SelectList? AvailableLocations { get; set; }
        public SelectList? AvailableFridgeModels { get; set; }
    }
}
