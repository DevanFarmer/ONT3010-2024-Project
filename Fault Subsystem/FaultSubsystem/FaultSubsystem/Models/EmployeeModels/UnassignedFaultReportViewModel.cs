namespace FaultSubsystem.Models.EmployeeModels
{
    public class UnassignedFaultReportViewModel
    {
        public int FaultID { get; set; }
        public string FaultDescription { get; set; }
        public string FaultStatus { get; set; }
        public string FridgeModel { get; set; }
        public string SerialNumber { get; set; }
        public string AllocationDate { get; set; }
    }
}
