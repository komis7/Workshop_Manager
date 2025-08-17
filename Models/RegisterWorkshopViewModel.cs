using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using WorkShopManager.Validators;

namespace WorkShopManager.Models
{
    public class RegisterWorkshopViewModel
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

        [Required(ErrorMessage = "Podanie nazwy firmy jest wymagane.")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Podanie nazwy ulicy jest wymagane.")]
        [RegularExpression(@"^[A-Za-zĄĆĘŁŃÓŚŹŻąćęłńóśźż0-9\s]+$", ErrorMessage = "Nazwa ulicy może zawierać tylko litery, cyfry i spacje.")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Podanie numeru budynku jest wymagane.")]
        [RegularExpression(@"^[1-9]+$", ErrorMessage = "Numer budynku musi być liczbą większą od zera.")]
        public string BuildingNumber { get; set; }

        [Required(ErrorMessage = "Podanie kodu pocztowego jest wymagane.")]
        [RegularExpression(@"\d{2}-\d{3}", ErrorMessage = "Kod pocztowy musi być w formacie XX-XXX.")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Podanie nazwy miasta jest wymagane.")]
        [RegularExpression(@"^[A-Za-zĄĆĘŁŃÓŚŹŻąćęłńóśźż]+$", ErrorMessage = "Miasto może zawierać tylko litery.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Podanie NIP-u jest wymagane.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "NIP musi zawierać 10 cyfr.")]
        public string CompanyNIP { get; set; }

        [Required(ErrorMessage = "Podanie numeru telefonu jest wymagane.")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Numer telefonu musi posiadać dokładnie 9 cyfr.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Należy podać liczbę stanowisk.")]
        [Range(1, 10, ErrorMessage = "Minimalna liczba stanowisk to 1.")]
        public int WorkshopSlots { get; set; }

        public List<string> SelectedServices { get; set; } = new List<string>();

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
        [Required(ErrorMessage = "Proszę podać stawkę godzinową.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Stawka godzinowa musi być większa od 0.")]
        public decimal HourlyRate { get; set; }
    }
}

