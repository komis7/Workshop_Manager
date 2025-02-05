namespace WorkShopManager.Models
{
    public class Appointment
    {
        public int Id { get; set; } // Klucz główny
        public string CustomerName { get; set; } // Imię i nazwisko klienta
        public string PhoneNumber { get; set; } // Telefon kontaktowy
        public string CarModel { get; set; } // Model samochodu
        public DateTime AppointmentDate { get; set; } // Data wizyty
        public string ServiceType { get; set; } // Typ usługi (np. przegląd, naprawa)
    
    }
}
