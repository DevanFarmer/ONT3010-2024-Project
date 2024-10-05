namespace FaultSubsystem.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public int UserID { get; set; }

        public User User { get; set; }
    }
}
