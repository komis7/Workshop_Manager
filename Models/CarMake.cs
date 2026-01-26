namespace WorkShopManager.Models
{
    public class CarMake
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<CarModel> Models { get; set; } = new List<CarModel>();
    }
}
