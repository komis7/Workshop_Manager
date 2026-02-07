namespace WorkShopManager.Models
{
    public class ClientVisitHistoryItemVm
    {
        public int EventId { get; set; }
        public DateTime Start { get; set; }

        public string WorkshopId { get; set; } = "";
        public string WorkshopName { get; set; } = "";
        public string? WorkshopAddress { get; set; }

        public string ServiceTitle { get; set; } = "";
        public string? WorkshopNote { get; set; }
        public string? VehicleMake { get; set; }
        public string? VehicleModel { get; set; }
        public int? VehicleYear { get; set; }
        public string? VehiclePlate { get; set; }
        public string? VehicleVin { get; set; }

    }
}
