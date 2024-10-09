namespace FaultSubsystem.Models
{
    public class FridgeAllocation
    {
        public int AllocationID { get; set; }
        public int FridgeID { get; set; }
        public int CustomerID { get; set; }
        public DateTime AllocationDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public Fridge Fridge { get; set; }
        public Fridge Customer { get; set; }
    }
}
