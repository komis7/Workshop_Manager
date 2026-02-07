using Newtonsoft.Json.Linq;
using WorkShopManager.Models;

public class CarQueryService
{
    public async Task<List<CarMake>> GetAllFromLocalJsonAsync()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "car_data.json");
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Plik JSON nie został znaleziony.");
            return new List<CarMake>();
        }

        var json = await File.ReadAllTextAsync(filePath);
        var dataArray = JArray.Parse(json);

        var makeList = dataArray.Select(x => new CarMake
        {
            Name = x["make"].ToString(),
            Models = new List<CarModel> { new CarModel { Name = x["model"].ToString() } }
        })
        .GroupBy(m => m.Name)
        .Select(g => new CarMake
        {
            Name = g.Key,
            Models = g.SelectMany(m => m.Models)
                      .GroupBy(m2 => m2.Name)
                      .Select(m3 => m3.First())
                      .ToList()
        })
        .ToList();

        Console.WriteLine($"Załadowano {makeList.Count} marek i łączną liczbę modeli: {makeList.Sum(m => m.Models.Count)}");

        return makeList;
    }
}
