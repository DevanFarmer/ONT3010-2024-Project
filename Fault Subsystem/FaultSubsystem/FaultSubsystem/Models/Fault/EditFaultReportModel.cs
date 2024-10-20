using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace FaultSubsystem.Models.Fault
{
    public class EditFaultReportModel
    {
        public int FaultID { get; set; }

        [StringLength(255, MinimumLength = 3, ErrorMessage = "First Name must be between 3 and 255 characters")]
        public string? Diagnosis { get; set; } // Allow employee to input diagnosis

        [Required(ErrorMessage = "Please select a Fault Status.")]
        public int FaultStatusID { get; set; } // Store the selected status

        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date.")]
        public DateTime? ScheduledRepairDate { get; set; }

        [StringLength(255, MinimumLength = 3, ErrorMessage = "First Name must be between 3 and 255 characters")]
        public string? Notes { get; set; }
        
        public List<FaultStatus> AvailableStatuses { get; set; } // Dropdown options
    }
}
