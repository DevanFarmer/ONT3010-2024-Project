using System.ComponentModel.DataAnnotations;

namespace FaultSubsystem.Models
{
    public class Supplier
    {
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Supplier Name is required")]
        [StringLength(55, MinimumLength = 3, ErrorMessage = "First Name must be between 3 and 55 characters")]
        public string SupplierName { get; set;}

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string SupplierPhoneNumber { get; set;}
    }
}
