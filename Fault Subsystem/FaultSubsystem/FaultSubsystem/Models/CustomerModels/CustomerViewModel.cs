using System.ComponentModel.DataAnnotations;

namespace FaultSubsystem.Models.CustomerModels
{
    public class CustomerViewModel
    {
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Last Name must be between 3 and 100 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Last Name must be between 3 and 100 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }
    }
}
