namespace FaultSubsystem.Models.FridgeRequestModels
{
    public class ViewCustomerFridgeRequests
    {
        public int FridgeRequestID { get; set; }
        public string FridgeModel { get; set; }
        public int? AssignFridgeID { get; set; }  // Nullable as it might not yet be assigned
        public bool Handled { get; set; }
    }
}
