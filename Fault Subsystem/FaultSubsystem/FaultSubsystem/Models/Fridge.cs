using System.ComponentModel.DataAnnotations;

namespace FaultSubsystem.Models
{
    public class Fridge
    {
        [Key]
        public int FridgeID { get; set; }
        [Required]
        public int FridgeTypeID { get; set; }
        [Required]
        public int StatusID { get; set; }
        [Required]
        public int LocationID { get; set; }
        [Required]
        public string SerialNumber { get; set; }
        [Required]
        public DateTime DateAcquired { get; set; }

        public Inventory Inventory { get; set; }
        public Status Status { get; set; }
        public Location Location { get; set; }
        public ICollection<FridgeAllocation> FridgeAllocation { get; set; }
    }
}
