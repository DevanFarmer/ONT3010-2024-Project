using System.ComponentModel.DataAnnotations;

namespace FaultSubsystem.Models.CustomerModels
{
    public class FaultReportViewModel
    {
        public int AllocationID { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "The fault description cannot exceed 255 characters.")]
        public string FaultDescription { get; set; }
    }
}
