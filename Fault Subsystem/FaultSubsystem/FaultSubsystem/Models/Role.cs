using System.ComponentModel.DataAnnotations;

namespace FaultSubsystem.Models
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }
        [Required(ErrorMessage = "Role Name is required")]
        [StringLength(55, MinimumLength = 3, ErrorMessage = "First Name must be between 3 and 55 characters")]
        public string RoleName { get; set; }
    }
}
