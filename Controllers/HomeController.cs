using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkShopManager.Data;
using WorkShopManager.Models;

namespace WorkShopManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WorkshopContext _context; // Dodano kontekst bazy danych

        public HomeController(ILogger<HomeController> logger, WorkshopContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                var roles = _context.UserRoles.Where(ur => ur.UserId == user.Id).Select(ur => ur.RoleId).ToList();
                var workshopRoleId = "3bd7d0c5-f004-4ee2-9ea9-b49ce540e67b"; // Rola warsztatu

                if (roles.Contains(workshopRoleId))
                {
                    // Widok dla warsztatu
                    return View("IndexWorkshop", user);
                }
            }

            var workshops = _context.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == "3bd7d0c5-f004-4ee2-9ea9-b49ce540e67b"))
                .Select(w => new
                {
                    w.Id,
                    w.CompanyName,
                    w.PhoneNumber,
                    Address = $"{w.Street} {w.BuildingNumber}, {w.PostalCode} {w.City}",
                    Services = w.Services
                })
                .ToList();

            ViewBag.Workshops = workshops;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
