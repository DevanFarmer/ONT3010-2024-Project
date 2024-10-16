using System.ComponentModel.DataAnnotations;

namespace FaultSubsystem.Models
{
    public class Inventory
    {
        [Key]
        public int FridgeTypeID { get; set; }
        [Required]
        public int SupplierID { get; set; }
        [Required]
        public string FridgeModel { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
