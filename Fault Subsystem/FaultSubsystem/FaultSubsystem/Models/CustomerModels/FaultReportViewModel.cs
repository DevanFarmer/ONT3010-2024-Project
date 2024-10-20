using System.ComponentModel.DataAnnotations;

namespace FaultSubsystem.Models.CustomerModels
{
    public class FaultReportViewModel
    {
        public int AllocationID { get; set; }

        [Required(ErrorMessage = "Please describe the fault you are experiencing.")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "First Name must be between 3 and 255 characters")]
        public string FaultDescription { get; set; }
    }
}
