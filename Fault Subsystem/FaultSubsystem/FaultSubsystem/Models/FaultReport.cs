namespace FaultSubsystem.Models
{
    public class FaultReport
    {
        public int FaultID { get; set; }
        public int AllocationID { get; set; }
        public int? EmployeeID { get; set; }
        public int FaultStatusID { get; set; }
        public string FaultDescription { get; set; }
        public DateTime ReportDate { get; set; }
        public DateTime? ResolutionDate { get; set; }
        public DateTime? AssignedDate { get; set; }
        public DateTime? ScheduledRepairDate { get; set; }
        public string? Diagnosis { get; set; }
        public string? Notes { get; set; }

        public FridgeAllocation FridgeAllocation { get; set; }
        public Employee Employee { get; set; }
        public FaultStatus FaultStatus { get; set; }
    }
}
