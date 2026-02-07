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

        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            var workshop = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (workshop == null)
                return NotFound();

            var model = new WorkshopEditProfileViewModel
            {
                CompanyName = workshop.CompanyName,
                Street = workshop.Street,
                BuildingNumber = workshop.BuildingNumber ?? "",
                PostalCode = workshop.PostalCode,
                City = workshop.City,
                CompanyNIP = workshop.CompanyNIP,
                PhoneNumber = workshop.PhoneNumber,
                //WorkshopSlots = workshop.WorkshopSlots ?? 1,
                HourlyRate = workshop.HourlyRate ?? 0,
                SelectedServices = string.IsNullOrWhiteSpace(workshop.Services)
                    ? new List<string>()
                    : workshop.Services.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .ToList(),
                CustomServices = string.IsNullOrWhiteSpace(workshop.CustomServicesCsv)
                ? new List<string>()
                : workshop.CustomServicesCsv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()).ToList()

            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dashboard(WorkshopEditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            var workshop = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (workshop == null)
                return NotFound();

            workshop.CompanyName = model.CompanyName;
            workshop.Street = model.Street;
            workshop.BuildingNumber = model.BuildingNumber.ToString();
            workshop.PostalCode = model.PostalCode;
            workshop.City = model.City;
            workshop.CompanyNIP = model.CompanyNIP;
            workshop.PhoneNumber = model.PhoneNumber;
            //workshop.WorkshopSlots = model.WorkshopSlots;
            workshop.HourlyRate = model.HourlyRate;

            workshop.Services = model.SelectedServices != null && model.SelectedServices.Any()
                ? string.Join(", ", model.SelectedServices)
                : "";
            // stała lista custom (zawsze checkboxy własne + te dodane w tej sesji)
            workshop.CustomServicesCsv = model.CustomServices != null && model.CustomServices.Any()
                ? string.Join(", ", model.CustomServices.Distinct(StringComparer.OrdinalIgnoreCase))
                : "";

            await _context.SaveChangesAsync();
            TempData["SavedOk"] = "Dane warsztatu zostały zaktualizowane.";
            return RedirectToAction(nameof(Dashboard));
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
                    Start = e.Start,
                    End = e.Start.AddHours(1),
                    VehicleMake = User.IsInRole("Workshop") ? e.VehicleMake : null,
                    VehicleModel = User.IsInRole("Workshop") ? e.VehicleModel : null,
                    VehicleYear = User.IsInRole("Workshop") ? (int?)e.VehicleYear : null,
                    VehiclePlate = User.IsInRole("Workshop") ? e.VehiclePlate : null,
                    VehicleVin = User.IsInRole("Workshop") ? e.VehicleVin : null,
                    CustomerPhone = User.IsInRole("Workshop") ? e.CustomerPhone : null,
                    CustomerEmail = User.IsInRole("Workshop") ? e.CustomerEmail : null,
                    Status = e.Status,
                    WorkshopNote = User.IsInRole("Workshop") ? e.WorkshopNote : null,
                    FaultDescription = User.IsInRole("Workshop") ? e.FaultDescription : null
                })
                .ToListAsync();

            return Json(events);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddEvent([FromBody] AddEventDto dto)
        {

            dto.VehiclePlate = string.IsNullOrWhiteSpace(dto.VehiclePlate)
                ? null
                : dto.VehiclePlate.Replace(" ", "").Replace("-", "").ToUpperInvariant();

            dto.VehicleVin = string.IsNullOrWhiteSpace(dto.VehicleVin)
                ? null
                : dto.VehicleVin.Replace(" ", "").ToUpperInvariant();

            dto.CustomerPhone = dto.CustomerPhone.Trim();

            var phoneDigits = new string(dto.CustomerPhone.Where(char.IsDigit).ToArray());

            if (phoneDigits.Length < 9)
            {
                return BadRequest(new
                {
                    errors = new
                    {
                        CustomerPhone = new[] { "Numer telefonu musi zawierać co najmniej 9 cyfr." }
                    }
                });
            }


            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    k => k.Key,
                    v => v.Value!.Errors.Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Niepoprawna wartość." : e.ErrorMessage).ToArray()
                );

                return BadRequest(new { errors });
            }

            if (string.IsNullOrEmpty(dto.WorkshopId))
                return BadRequest(new { errors = new { WorkshopId = new[] { "Identyfikator warsztatu jest wymagany." } } });

            var workshop = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.WorkshopId);
            if (workshop == null)
                return BadRequest(new { message = "Nie znaleziono warsztatu." });

            if (dto.VehicleYear.HasValue)
            {
                var maxYear = DateTime.Now.Year + 1;
                if (dto.VehicleYear < 1950 || dto.VehicleYear > maxYear)
                    return BadRequest(new { errors = new { VehicleYear = new[] { "Nieprawidłowy rok produkcji." } } });
            }


            var exists = await _context.Events.AnyAsync(e =>
                e.WorkshopId == dto.WorkshopId &&
                e.Start == dto.Start
            );

            if (exists)
                return BadRequest(new { errors = new { Start = new[] { "Ten termin jest już zajęty." } } });

            var newEvent = new Event
            {
                WorkshopId = dto.WorkshopId,
                Title = dto.Title,
                Start = dto.Start,

                VehicleMake = dto.VehicleMake,
                VehicleModel = dto.VehicleModel,
                VehicleYear = dto.VehicleYear ?? 0,
                VehiclePlate = dto.VehiclePlate,
                VehicleVin = dto.VehicleVin,
                CustomerPhone = dto.CustomerPhone,
                CustomerEmail = dto.CustomerEmail,
                FaultDescription = string.IsNullOrWhiteSpace(dto.FaultDescription) ? null : dto.FaultDescription.Trim()
            };

            // sprawdzamy dzień otwarcia warsztatu (workshopId)
            var openDaysCsv = workshop.OpenDaysCsv;
            var openDays = string.IsNullOrWhiteSpace(openDaysCsv)
                ? new HashSet<int>(new[] { 1, 2, 3, 4, 5, 6 }) // domyślnie pn-sob
                : new HashSet<int>(
                    openDaysCsv.Split(',')
                        .Select(s => int.TryParse(s.Trim(), out var n) ? n : -1)
                        .Where(n => n >= 0 && n <= 6)
                );

            var day = (int)newEvent.Start.DayOfWeek; // niedz=0 ... sob=6
            if (!openDays.Contains(day))
            {
                return BadRequest(new
                {
                    success = false,
                    errors = new
                    {
                        EventDate = new[] { "Warsztat jest zamknięty w wybrany dzień." }
                    }
                });
            }

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null && await _userManager.IsInRoleAsync(user, "Client"))
                {
                    newEvent.ClientId = user.Id;
                }
            }


            newEvent.Status = VisitStatus.New;
            newEvent.WorkshopNote = (newEvent.WorkshopNote ?? "").Trim();

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
                return NotFound(new { success = false, message = "Wizyta nie została znaleziona." });

            // 1) Zakończona = brak edycji
            if (eventInDb.Status == VisitStatus.Done)
            {
                return BadRequest(new { success = false, message = "Wizyta jest zakończona i nie można jej edytować." });
            }

            // 2) Jeśli w trakcie: tylko opis + ewentualnie przejście na Done
            if (eventInDb.Status == VisitStatus.InProgress)
            {
                // dozwolone: aktualizacja opisu
                eventInDb.WorkshopNote = (updatedEvent.WorkshopNote ?? "").Trim();

                // dozwolone: zmiana statusu InProgress -> Done
                if (updatedEvent.Status == VisitStatus.Done)
                    eventInDb.Status = VisitStatus.Done;

                await _context.SaveChangesAsync();
                return Ok(new { success = true });
            }

            // 3) Jeśli New: wolno edytować pola + wolno zmienić status na InProgress lub Done (jeśli chcesz)
            eventInDb.Title = updatedEvent.Title;
            eventInDb.Start = updatedEvent.Start;
            eventInDb.VehicleMake = updatedEvent.VehicleMake;
            eventInDb.VehicleModel = updatedEvent.VehicleModel;
            eventInDb.VehicleYear = updatedEvent.VehicleYear;
            eventInDb.VehiclePlate = updatedEvent.VehiclePlate;
            eventInDb.VehicleVin = updatedEvent.VehicleVin;
            eventInDb.CustomerPhone = updatedEvent.CustomerPhone;
            eventInDb.CustomerEmail = updatedEvent.CustomerEmail;

            eventInDb.WorkshopNote = (updatedEvent.WorkshopNote ?? "").Trim();

            // statusy: New -> InProgress lub Done
            if (updatedEvent.Status == VisitStatus.InProgress || updatedEvent.Status == VisitStatus.Done)
                eventInDb.Status = updatedEvent.Status;

            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }


        [HttpPost]
        [Authorize(Roles = "Workshop")]
        public async Task<IActionResult> SaveWorkHours([FromBody] WorkHoursDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Min) || string.IsNullOrWhiteSpace(dto.Max))
                return BadRequest(new { success = false, message = "Brak danych." });

            if (!TimeSpan.TryParse(dto.Min, out var min) || !TimeSpan.TryParse(dto.Max, out var max))
                return BadRequest(new { success = false, message = "Nieprawidłowy format godziny." });

            if (min >= max)
                return BadRequest(new { success = false, message = "Godzina rozpoczęcia musi być wcześniejsza niż zakończenia." });

            // zapisujemy godziny ZAWSZE dla zalogowanego warsztatu (żeby nie dało się zmienić komuś innemu)
            var userId = _userManager.GetUserId(User);
            var workshop = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (workshop == null)
                return NotFound(new { success = false, message = "Nie znaleziono warsztatu." });

            workshop.WorkStart = min;
            workshop.WorkEnd = max;

            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }
        public class OpenDaysDto
        {
            public int[] Days { get; set; } = Array.Empty<int>(); // wartości 0..6
        }
        [HttpPost]
        [Authorize(Roles = "Workshop")]
        public async Task<IActionResult> SaveOpenDays([FromBody] OpenDaysDto dto)
        {
            if (dto?.Days == null || dto.Days.Length == 0)
                return BadRequest(new { success = false, message = "Wybierz co najmniej jeden dzień otwarcia." });

            // walidacja zakresu
            if (dto.Days.Any(d => d < 0 || d > 6))
                return BadRequest(new { success = false, message = "Nieprawidłowe dni tygodnia." });

            // zapisujemy dla zalogowanego warsztatu
            var userId = _userManager.GetUserId(User);
            var workshop = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (workshop == null)
                return NotFound(new { success = false, message = "Nie znaleziono warsztatu." });

            // zapis CSV, unikalnie i posortowane
            var cleaned = dto.Days.Distinct().OrderBy(x => x).ToArray();
            workshop.OpenDaysCsv = string.Join(",", cleaned);

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
            ViewBag.WorkshopServices = workshop.Services ?? "";
            ViewBag.WorkStart = workshop.WorkStart?.ToString(@"hh\:mm") ?? "08:00";
            ViewBag.WorkEnd = workshop.WorkEnd?.ToString(@"hh\:mm") ?? "18:00";
            ViewBag.OpenDays = string.IsNullOrWhiteSpace(workshop.OpenDaysCsv)
            ? "1,2,3,4,5,6" // domyślnie pn-sob
            : workshop.OpenDaysCsv;


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
                    ViewBag.VehiclePlate = user.VehicleDetails?.Split(',')[3]?.Trim();
                    ViewBag.VehicleVin = user.VehicleDetails?.Split(',')[4]?.Trim();
                }
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VisitHistory()
        {
            var workshopId = _userManager.GetUserId(User);

            var done = VisitStatus.Done; // jeśli Status jest enum
                                         // jeśli Status jest int, użyj:
                                         // var done = 2;

            var visits = await _context.Events
                .Where(e => e.WorkshopId == workshopId && e.Status == done)
                .OrderByDescending(e => e.Start)
                .Select(e => new WorkShopVisitHistoryItemVm
                {
                    EventId = e.Id,
                    Start = e.Start,
                    ServiceTitle = e.Title,

                    ClientEmail = e.CustomerEmail,
                    ClientPhone = e.CustomerPhone,

                    VehicleMake = e.VehicleMake,
                    VehicleModel = e.VehicleModel,
                    VehicleYear = e.VehicleYear == 0 ? null : e.VehicleYear,
                    VehiclePlate = e.VehiclePlate,
                    VehicleVin = e.VehicleVin,

                    WorkshopNote = e.WorkshopNote
                })
                .ToListAsync();

            return View(visits);
        }


        public class WorkHoursDto
        {
            public string Min { get; set; } = "";
            public string Max { get; set; } = "";
        }



    }
}