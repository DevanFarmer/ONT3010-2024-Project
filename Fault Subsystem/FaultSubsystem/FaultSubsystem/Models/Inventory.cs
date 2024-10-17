using System.ComponentModel.DataAnnotations;

namespace FaultSubsystem.Models
{
    public class Inventory
    {
        public int FridgeTypeID { get; set; }
        public int SupplierID { get; set; }
        public string FridgeModel { get; set; }
        public int Quantity { get; set; }

        public Supplier Supplier { get; set; }
        public ICollection<Fridge> Fridge { get; set; }
    }
}
