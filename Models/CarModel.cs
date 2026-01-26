namespace WorkShopManager.Models
{
    public class CarModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int CarMakeId { get; set; }
        public CarMake CarMake { get; set; }
    }
}
