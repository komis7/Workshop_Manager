using System.ComponentModel.DataAnnotations;
using WorkShopManager.Validators;

namespace WorkShopManager.Models
{
    public class RegisterClientViewModel
    {
        [Required(ErrorMessage = "Adres e-mail jest wymagany.")]
        [EmailValidation]
        public string Email { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$",
            ErrorMessage = "Hasło musi mieć co najmniej 8 znaków, zawierać wielką i małą literę, cyfrę oraz znak specjalny.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Powtórzenie hasła jest wymagane.")]
        [Compare("Password", ErrorMessage = "Hasła muszą być identyczne.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Podanie numeru telefonu jest wymagane.")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Numer telefonu musi posiadać dokładnie 9 cyfr.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Marka pojazdu jest wymagana.")]
        public int SelectedMakeId { get; set; }

        [Required(ErrorMessage = "Model pojazdu jest wymagany.")]
        public int SelectedModelId { get; set; }

        public List<CarMake>? Makes { get; set; }
        public List<CarModel>? Models { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Rok produkcji musi mieć 4 cyfry.")]
        [Range(1950, 2100, ErrorMessage = "Podaj poprawny rok produkcji.")]
        public string VehicleYear { get; set; }

        [Required(ErrorMessage = "Podanie numeru rejestracyjnego pojazdu jest wymagane.")]
        [StringLength(8, MinimumLength = 5, ErrorMessage = "Numer rejestracyjny musi mieć od 5 do 8 znaków.")]
        public string VehicleRegistrationNumber { get; set; }

        [Required(ErrorMessage = "Numer VIN jest wymagany.")]
        [StringLength(17, MinimumLength = 17, ErrorMessage = "Numer VIN musi mieć dokładnie 17 znaków.")]
        public string VehicleVIN { get; set; }
    }
}
