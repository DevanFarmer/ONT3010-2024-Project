namespace FaultSubsystem.Models.Fault
{
    public class EditFaultReportModel
    {
        public int FaultID { get; set; }
        public string Diagnosis { get; set; } // Allow employee to input diagnosis
        public int FaultStatusID { get; set; } // Store the selected status
        public DateTime? ScheduledRepairDate { get; set; }
        public string Notes { get; set; }
        public List<FaultStatus> AvailableStatuses { get; set; } // Dropdown options
    }
}
