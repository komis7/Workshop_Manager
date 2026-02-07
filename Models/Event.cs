using System;
using System.ComponentModel.DataAnnotations;

namespace WorkShopManager.Models
{

    public enum VisitStatus
    {
        New = 0,
        InProgress = 1,
        Done = 2
    }

    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public string WorkshopId { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public int VehicleYear { get; set; }
        public string VehiclePlate { get; set; }
        public string VehicleVin { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public VisitStatus Status { get; set; } = VisitStatus.New;
        public string? FaultDescription { get; set; } 
        public string? WorkshopNote { get; set; }
        public string? ClientId { get; set; }
    }
}
