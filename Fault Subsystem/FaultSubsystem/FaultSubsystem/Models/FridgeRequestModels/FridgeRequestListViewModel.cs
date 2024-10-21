namespace FaultSubsystem.Models.FridgeRequestModels
{
    public class FridgeRequestListViewModel
    {
        public int FridgeRequestID { get; set; }
        public string FridgeModel { get; set; }
        public bool Handled { get; set; }
        public int? AssignFridgeID { get; set; }
    }
}
