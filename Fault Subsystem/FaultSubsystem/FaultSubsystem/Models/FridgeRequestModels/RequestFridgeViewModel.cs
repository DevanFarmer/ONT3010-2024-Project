using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FaultSubsystem.Models.FridgeRequestModels
{
    public class RequestFridgeViewModel
    {
        [Required(ErrorMessage = "Please select a fridge model.")]
        [Display(Name = "Fridge Model")]
        public string? SelectedFridgeModel { get; set; }

        public List<SelectListItem>? AvailableFridgeModels { get; set; }
    }
}
