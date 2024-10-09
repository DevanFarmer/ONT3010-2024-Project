namespace FaultSubsystem.Models.CustomerModels
{
    public class AllocatedFridgesViewModel
    {
        public int FridgeID { get; set; }
        public string FridgeModel { get; set; }
        public string SerialNumber { get; set; }

        public string Addressline1 { get; set; }
        public string Addressline2 { get; set; }
        public string City { get; set;}
        public string PostalCode { get; set;}

        public int AllocationID { get; set; }
    }
}
