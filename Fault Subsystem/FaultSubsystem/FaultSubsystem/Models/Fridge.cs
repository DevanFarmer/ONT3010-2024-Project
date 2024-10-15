namespace FaultSubsystem.Models
{
    public class Fridge
    {
        public int FridgeID { get; set; }
        public int SupplierID { get; set; }
        public int StatusID { get; set; }
        public int LocationID { get; set; }
        public string FridgeModel { get; set; }
        public string SerialNumber { get; set; }
        public DateTime DateAcquired { get; set; }

        public Supplier Supplier { get; set; }
        public Status Status { get; set; }
        public Location Location { get; set; }
        public ICollection<FridgeAllocation> FridgeAllocation { get; set; }
    }
}
