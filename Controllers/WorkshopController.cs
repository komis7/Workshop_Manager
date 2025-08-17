using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkShopManager.Data;
using WorkShopManager.Models;

namespace WorkShopManager.Controllers
{
    [Authorize(Roles = "Workshop")]
    public class WorkshopController : Controller
    {
        private readonly WorkshopContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WorkshopController(WorkshopContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvents(string workshopId)
        {
            if (string.IsNullOrEmpty(workshopId))
            {
                return BadRequest(new { success = false, message = "Identyfikator warsztatu jest wymagany." });
            }

            var events = await _context.Events
                .Where(e => e.WorkshopId == workshopId)
                .Select(e => new
                {
                    e.Id,
                    e.Title,
                    e.Start,
                    VehicleMake = User.IsInRole("Workshop") ? e.VehicleMake : null,
                    VehicleModel = User.IsInRole("Workshop") ? e.VehicleModel : null,
                    VehicleYear = User.IsInRole("Workshop") ? (int?)e.VehicleYear : null,
                    VehiclePlate = User.IsInRole("Workshop") ? e.VehiclePlate : null,
                    VehicleVin = User.IsInRole("Workshop") ? e.VehicleVin : null,
                    CustomerPhone = User.IsInRole("Workshop") ? e.CustomerPhone : null,
                    CustomerEmail = User.IsInRole("Workshop") ? e.CustomerEmail : null
                })
                .ToListAsync();

            return Json(events);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddEvent([FromBody] Event newEvent)
        {
            if (string.IsNullOrEmpty(newEvent.WorkshopId))
            {
                return BadRequest(new { success = false, message = "Identyfikator warsztatu jest wymagany." });
            }

            var workshop = await _context.Users.FirstOrDefaultAsync(u => u.Id == newEvent.WorkshopId);
            if (workshop == null)
            {
                return NotFound(new { success = false, message = "Nie znaleziono warsztatu." });
            }

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }



        [HttpPost]
        public async Task<IActionResult> DeleteEvent([FromBody] Event eventToDelete)
        {
            var eventInDb = await _context.Events.FindAsync(eventToDelete.Id);
            if (eventInDb == null)
            {
                return NotFound(new { success = false, message = "Wizyta nie została znaleziona." });
            }

            _context.Events.Remove(eventInDb);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }


        [HttpPost]
        public async Task<IActionResult> UpdateEvent([FromBody] Event updatedEvent)
        {
            var eventInDb = await _context.Events.FindAsync(updatedEvent.Id);
            if (eventInDb == null)
            {
                return NotFound(new { success = false, message = "Wizyta nie została znaleziona." });
            }

            eventInDb.Title = updatedEvent.Title;
            eventInDb.Start = updatedEvent.Start;
            eventInDb.VehicleMake = updatedEvent.VehicleMake;
            eventInDb.VehicleModel = updatedEvent.VehicleModel;
            eventInDb.VehicleYear = updatedEvent.VehicleYear;
            eventInDb.VehiclePlate = updatedEvent.VehiclePlate;
            eventInDb.VehicleVin = updatedEvent.VehicleVin;
            eventInDb.CustomerPhone = updatedEvent.CustomerPhone;
            eventInDb.CustomerEmail = updatedEvent.CustomerEmail;

            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("Workshop/Schedule/{id}")]
        public async Task<IActionResult> Schedule(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Identyfikator warsztatu nie został podany.");
            }

            var workshop = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (workshop == null)
            {
                return NotFound("Nie znaleziono warsztatu.");
            }

            ViewBag.WorkshopName = workshop.CompanyName;
            ViewBag.WorkshopId = workshop.Id;

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null && await _userManager.IsInRoleAsync(user, "Client"))
                    {
                    ViewBag.UserPhoneNumber = user.PhoneNumber;
                    ViewBag.UserEmail = user.Email;
                    ViewBag.VehicleMake = user.VehicleDetails?.Split(',')[0];
                    ViewBag.VehicleModel = user.VehicleDetails?.Split(',')[1];
                    ViewBag.VehicleYear = user.VehicleDetails?.Split(',')[2];
                    ViewBag.VehiclePlate = user.VehicleDetails?.Split(',')[3];
                    ViewBag.VehicleVin = user.VehicleDetails?.Split(',')[4];
                }
            }

            return View();
        }
    }
}