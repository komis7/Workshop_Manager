using System.ComponentModel.DataAnnotations;

namespace WorkShopManager.Models
{
    public class AddEventDto
    {
        [Required]
        public string WorkshopId { get; set; } = "";

        [Required(ErrorMessage = "Wybierz usługę.")]
        public string Title { get; set; } = "";

        [Required(ErrorMessage = "Wybierz datę i godzinę.")]
        public DateTime Start { get; set; }

        [Required(ErrorMessage = "Podaj numer telefonu.")]
        [MinLength(9, ErrorMessage = "Numer telefonu musi zawierać co najmniej 9 cyfr.")]
        [RegularExpression(@"^[0-9+\s-]+$", ErrorMessage = "Numer telefonu zawiera niedozwolone znaki.")]
        public string CustomerPhone { get; set; } = "";

        [Required(ErrorMessage = "Podaj email.")]
        [EmailAddress(ErrorMessage = "Podaj poprawny email.")]
        public string CustomerEmail { get; set; } = "";

        [Required(ErrorMessage = "Podaj rok produkcji.")]
        [Range(1950, 2100, ErrorMessage = "Podaj poprawny rok produkcji.")]
        public int? VehicleYear { get; set; }

        public string? VehicleMake { get; set; }
        public string? VehicleModel { get; set; }
        [Required(ErrorMessage = "Podaj numer rejestracyjny.")]
        [RegularExpression(@"^[A-Za-z0-9]{4,9}$", ErrorMessage = "Podaj poprawny numer rejestracyjny (4–9 znaków: litery/cyfry, bez spacji).")]
        public string? VehiclePlate { get; set; }
        [Required(ErrorMessage = "Podaj numer VIN.")]
        [RegularExpression(@"^[A-HJ-NPR-Za-hj-npr-z0-9]{17}$", ErrorMessage = "VIN musi mieć 17 znaków (litery/cyfry), bez liter I, O, Q.")]
        public string? VehicleVin { get; set; }
        [StringLength(1000, ErrorMessage = "Opis usterki może mieć maksymalnie 1000 znaków.")]
        public string? FaultDescription { get; set; }

    }
}
