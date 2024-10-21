namespace FaultSubsystem.Models.CustomerModels
{
    public class FaultyFridgesViewModel
    {
        public int FridgeID { get; set; }
        public string FridgeModel { get; set; }
        public string SerialNumber { get; set; }

        public int FaultID { get; set; }

        public string FaultDescription { get; set; }
        public string FaultStatus { get; set; }
        public string? ReturnDate { get; set; }
    }
}
