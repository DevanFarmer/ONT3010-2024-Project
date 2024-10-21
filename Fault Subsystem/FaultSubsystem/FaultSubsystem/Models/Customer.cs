namespace FaultSubsystem.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public int UserID { get; set; }

        public User User { get; set; }
        public ICollection<FridgeAllocation> FridgeAllocation { get; set; }
        public ICollection<FridgeRequest> FridgeRequest { get; set; }
    }
}
