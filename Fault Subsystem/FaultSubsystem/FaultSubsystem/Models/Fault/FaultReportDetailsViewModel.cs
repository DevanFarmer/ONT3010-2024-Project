namespace FaultSubsystem.Models.Fault
{
    public class FaultReportDetailsViewModel
    {
        public int FaultID { get; set; }
        public string FaultDescription { get; set; }
        public string FaultStatus { get; set; }
        public string ReportDate { get; set; }
        public string Diagnosis { get; set; }
        public string ScheduledRepairDate { get; set; }
        public string Notes { get; set; }
        public string ResolutionDate { get; set; }
        public string FridgeModel { get; set; }
        public string SerialNumber { get; set; }
        public string DateAcquired { get; set; }
    }
}
