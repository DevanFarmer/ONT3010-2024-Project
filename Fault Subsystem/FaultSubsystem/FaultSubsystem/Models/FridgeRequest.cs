using System.ComponentModel.DataAnnotations;

namespace FaultSubsystem.Models
{
    public class FridgeRequest
    {
        public int FridgeRequestID { get; set; }
        public int CustomerID { get; set; }

        //[Required(ErrorMessage = "Please indicate the fridge model.")]
        //[StringLength(50, MinimumLength = 3, ErrorMessage = "First Name must be between 3 and 50 characters")]
        public string FridgeModel { get; set; }
        public bool Handled { get; set; }
        public int? AssignFridgeID { get; set; }

        public Customer Customer { get; set; }
        public Fridge Fridge { get; set; }
    }
}
