using System.ComponentModel.DataAnnotations;

namespace FaultSubsystem.Models
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
}
