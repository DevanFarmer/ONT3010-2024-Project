namespace FaultSubsystem.Models.Fault
{
    public class FaultReportTechnicianViewModel
    {
        public int FaultID { get; set; }
        public string FaultDescription { get; set; }
        public string ReportDate { get; set; }
        public string FaultStatus { get; set; }
        public string? AssignedDate { get; set; }
        public string? ScheduledRepairDate { get; set; }
        public string? Diagnosis { get; set; }
    }
}
