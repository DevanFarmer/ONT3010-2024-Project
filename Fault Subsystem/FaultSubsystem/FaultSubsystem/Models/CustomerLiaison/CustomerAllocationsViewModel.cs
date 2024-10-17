namespace FaultSubsystem.Models.CustomerLiaison
{
    public class CustomerAllocationsViewModel
    {
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public ErrorMessages ErrorMessage { get; set; }

        public List<AllocationViewModel> Allocations { get; set; }

    }

    public class AllocationViewModel
    {
        public string FridgeModel { get; set; }
        public string SerialNumber { get; set; }
        public string AllocationDate { get; set; }
        public string ReturnDate { get; set; }
    }

    public enum ErrorMessages
    {
        CustomerNotFound,
        NoAllocations,
        FridgeNotFound,
        NoFridgesFound
    }
}
