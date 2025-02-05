using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using WorkShopManager.Validators;

namespace WorkShopManager.Models
{
    public class RegisterWorkshopViewModel
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
        public string CompanyName { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Nazwa ulicy może zawierać tylko litery.")]
        public string Street { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Numer budynku musi być liczbą większą od zera.")]
        public int BuildingNumber { get; set; }

        [Required]
        [RegularExpression(@"\d{2}-\d{3}", ErrorMessage = "Kod pocztowy musi być w formacie XX-XXX.")]
        public string PostalCode { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Miasto może zawierać tylko litery.")]
        public string City { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "NIP musi zawierać 10 cyfr.")]
        public string CompanyNIP { get; set; }

        [Required]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Numer telefonu musi zawierać dokładnie 9 cyfr.")]
        public string PhoneNumber { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Ilość stanowisk musi być od 1 do 100.")]
        public int WorkshopSlots { get; set; }

        // Usługi wybrane przez użytkownika
        public List<string> SelectedServices { get; set; } = new List<string>();

        // Wszystkie dostępne usługi (predefiniowane)
        public static List<string> AvailableServices { get; } = new List<string>
        {
            "Naprawa układu hamulcowego",
            "Naprawa zawieszenia",
            "Przegląd techniczny",
            "Wymiana oleju",
            "Diagnostyka",
            "Diagnostyka komputerowa",
            "Elektryka i elektronika",
            "Geometria",
            "Wulkanizacja",
            "Naprawa silnika",
            "Naprawy blacharskie",
            "Lakierowanie",
            "ChipTuning",
            "Tuning mechaniczny",
            "Tuning wizualny"
        
        };
        [Required]
        [Range(50, 1000, ErrorMessage = "Stawka godzinowa musi być w zakresie od 50 do 1000.")]
        public decimal HourlyRate { get; set; }
    }
}

