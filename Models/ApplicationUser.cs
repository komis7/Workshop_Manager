using Microsoft.AspNetCore.Identity;


namespace WorkShopManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? CompanyName { get; set; }
        public string? Street { get; set; }
        public string? BuildingNumber { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }
        public string? Services { get; set; } // Lista usług jako string
        public string? CustomServicesCsv { get; set; }

        public string? VehicleDetails { get; set; } // Szczegóły pojazdu, tylko dla klientów
        public int? HourlyRate { get; set; } // Stawka godzinowa, tylko dla warsztatów
        public TimeSpan? WorkStart { get; set; }
        public TimeSpan? WorkEnd { get; set; } 
        public string? OpenDaysCsv { get; set; }
        public string? CompanyNIP { get; set; }
        public int? WorkshopSlots { get; set; }
    }
}
