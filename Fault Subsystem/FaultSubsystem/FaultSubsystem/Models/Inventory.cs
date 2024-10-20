using System.ComponentModel.DataAnnotations;

namespace FaultSubsystem.Models
{
    public class Inventory
    {
        public int FridgeTypeID { get; set; }

        [Required(ErrorMessage = "Please select a Supplier")]
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "A Model Name is required")]
        [StringLength(55, MinimumLength = 3, ErrorMessage = "First Name must be between 3 and 55 characters")]
        public string FridgeModel { get; set; }

        [Required(ErrorMessage = "A quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid integer. Quantity cannot be lower than 0")]
        public int Quantity { get; set; }

        public Supplier Supplier { get; set; }
        public ICollection<Fridge> Fridge { get; set; }
    }
}
