using System.ComponentModel.DataAnnotations;

namespace FaultSubsystem.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public bool AccountActive { get; set; }

        public Customer Customer { get; set; }
        public Employee Employee { get; set; }
    }
}
