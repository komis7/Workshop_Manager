using System.ComponentModel.DataAnnotations;
using WorkShopManager.Validators;

namespace WorkShopManager.Models
{
    public class RegisterClientViewModel
    {
        [Required]
        [EmailValidation]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Hasło musi mieć co najmniej 8 znaków, zawierać wielką i małą literę, cyfrę oraz znak specjalny.")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Hasła muszą być identyczne.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Numer telefonu musi zawierać dokładnie 9 cyfr.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Marka pojazdu jest wymagana.")]
        public string VehicleBrand { get; set; }

        [Required(ErrorMessage = "Model pojazdu jest wymagany.")]
        public string VehicleModel { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Rok produkcji musi mieć 4 cyfry.")]
        public string VehicleYear { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 5, ErrorMessage = "Numer rejestracyjny musi mieć od 5 do 8 znaków.")]
        public string VehicleRegistrationNumber { get; set; }

        [Required(ErrorMessage = "Numer VIN jest wymagany.")]
        [StringLength(17, MinimumLength = 17, ErrorMessage = "Numer VIN musi mieć dokładnie 17 znaków.")]
        public string VehicleVIN { get; set; }
    }
}
