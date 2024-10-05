namespace FaultSubsystem.Models
{
    public class FridgeAllocation
    {
        public int AllocationID { get; set; }
        public int FridgeID { get; set; }
        public int CustomerID { get; set; }
        public int AllocationDate { get; set; }
        public int ReturnDate { get; set; }

        public Fridge Fridge { get; set; }
        public Fridge Customer { get; set; }
    }
}
