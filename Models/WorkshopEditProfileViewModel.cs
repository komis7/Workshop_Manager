using System.ComponentModel.DataAnnotations;
using WorkShopManager.Validators;

namespace WorkShopManager.Models
{
    public class WorkshopEditProfileViewModel
    {
        [Required(ErrorMessage = "Podanie nazwy firmy jest wymagane.")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Podanie nazwy ulicy jest wymagane.")]
        [RegularExpression(@"^[A-Za-zĄĆĘŁŃÓŚŹŻąćęłńóśźż0-9\s]+$", ErrorMessage = "Nazwa ulicy może zawierać tylko litery, cyfry i spacje.")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Podanie numeru budynku jest wymagane.")]
        [RegularExpression(@"^[1-9]\d*[A-Z]?(/([1-9]\d*))?$", ErrorMessage = "Numer budynku musi być liczbą większą od zera, opcjonalnie z dużą literą (np. 10A).")]
        public string BuildingNumber { get; set; } = "";

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

        //[Required(ErrorMessage = "Należy podać liczbę stanowisk.")]
        //[Range(1, 10, ErrorMessage = "Minimalna liczba stanowisk to 1.")]
        //public int WorkshopSlots { get; set; }

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
        public int HourlyRate { get; set; }
        public List<string> CustomServices { get; set; } = new List<string>();
        public string? NewService { get; set; }
        public string? RemoveService { get; set; } // opcjonalnie, jeśli chcesz usuwać custom z listy

    }
}


