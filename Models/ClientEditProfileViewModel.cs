using System.ComponentModel.DataAnnotations;
using WorkShopManager.Data;

namespace WorkShopManager.Models
{
    public class ClientEditProfileViewModel
    {
        public string PhoneNumber { get; set; }

        public string VehicleBrand { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleYear { get; set; }
        public string VehicleRegistrationNumber { get; set; }
        public string VehicleVIN { get; set; }
        [Required(ErrorMessage = "Marka pojazdu jest wymagana.")]
        public int? SelectedMakeId { get; set; }

        [Required(ErrorMessage = "Model pojazdu jest wymagany.")]
        public int? SelectedModelId { get; set; }
    }
}


