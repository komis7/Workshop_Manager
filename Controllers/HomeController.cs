using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ILogger<HomeController> logger, WorkshopContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var isWorkShop = await _userManager.IsInRoleAsync(user, "Workshop");

                if (!isWorkShop)
                {
                    // Widok dla warsztatu
                    return View("IndexWorkshop", user);
                }
            }
            
            var workshops = await (
                                                          from u in _context.Users
                                                          join ur in _context.UserRoles on u.Id equals ur.UserId
                                                          join r in _context.Roles on ur.RoleId equals r.Id
                                                          where r.Name == "Workshop"
                                                          select new
                                                          {
                                                              u.Id,
                                                              u.CompanyName,
                                                              u.PhoneNumber,
                                                              Address = $"{u.Street} {u.BuildingNumber}, {u.PostalCode} {u.City}",
                                                              Services = u.Services,
                                                              u.HourlyRate
                                                          }
                                                      ).ToListAsync();
                                          
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
