namespace FaultSubsystem.Models.CustomerModels
{
    public class FaultyFridgeDetailsViewModel
    {
        public int FridgeID { get; set; }
        public string FridgeModel { get; set; }
        public string SerialNumber { get; set; }

        public string Addressline1 { get; set; }
        public string Addressline2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

        public string AllocationDate { get; set; }
        public string? ReturnDate { get; set; }
        public FaultyFridgeFaultReportViewModel FaultReport { get; set; }
    }

    public class FaultyFridgeFaultReportViewModel
    {
        public string FaultDescription { get; set; }
        public string FaultStatus { get; set; }
        public string Diagnosis { get; set; }
        public string ScheduledRepairDate { get; set; }
        public string Notes { get; set; }
    }
}
