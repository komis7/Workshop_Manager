namespace WorkShopManager.Models
{
    public class WorkShopVisitHistoryItemVm
    {
        public int EventId { get; set; }
        public DateTime Start { get; set; }
        public string ServiceTitle { get; set; } = "";

        // Klient
        public string? ClientEmail { get; set; }
        public string? ClientPhone { get; set; }

        // Auto
        public string? VehicleMake { get; set; }
        public string? VehicleModel { get; set; }
        public int? VehicleYear { get; set; }
        public string? VehiclePlate { get; set; }
        public string? VehicleVin { get; set; }

        // Notatka warsztatu
        public string? WorkshopNote { get; set; }
    }
}
