using System.ComponentModel.DataAnnotations;

namespace FaultSubsystem.Models
{
    public class Location
    {
        public int LocationID { get; set; }
        
        [Required(ErrorMessage = "Address Line 1 is required")]
        [StringLength(55, MinimumLength = 3, ErrorMessage = "Address Line 1 must be between 3 and 55 characters")]
        public string AddressLine1 { get; set; }
        
        [StringLength(55, MinimumLength = 3, ErrorMessage = "Address Line 1 must be between 3 and 55 characters")]  
        public string? AddressLine2 { get; set; }
        
        [Required(ErrorMessage = "City Name is required")]
        [StringLength(55, MinimumLength = 3, ErrorMessage = "City must be between 3 and 55 characters")]
        public string City { get; set; }

        [Required(ErrorMessage = "Postal Code is required")]
        [StringLength(55, MinimumLength = 3, ErrorMessage = "Postal Code must be between 3 and 55 characters")]
        public string PostalCode { get; set; }
    }
}
