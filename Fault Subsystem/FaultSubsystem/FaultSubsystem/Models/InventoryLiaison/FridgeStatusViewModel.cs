using System.ComponentModel.DataAnnotations;

namespace FaultSubsystem.Models.InventoryLiaison
{
    public class FridgeStatusViewModel
    {
        public int FridgeStatusID { get; set; }

        [Required(ErrorMessage = "Status name is required.")]
        [StringLength(50, ErrorMessage = "Status name can't be longer than 50 characters.")]
        public string StatusName { get; set; }
    }
}
