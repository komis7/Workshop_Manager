using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkShopManager.Data;
using WorkShopManager.Models;
using Microsoft.EntityFrameworkCore;


namespace WorkShopManager.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly WorkshopContext _context;


        public ClientController(UserManager<ApplicationUser> userManager, WorkshopContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Dashboard()
        {
            return View(); // Główne menu
        }

        public async Task<IActionResult>  EditProfile()
        {
            var user =  await _userManager.GetUserAsync(User);

            var vehicleDetails = user.VehicleDetails?.Split(',').Select((value, index) => new { Value = value?.Trim(), Index = index }).ToList();

            var model = new ClientEditProfileViewModel
            {
                PhoneNumber = user.PhoneNumber,
                VehicleBrand = vehicleDetails?.FirstOrDefault(v => v.Index == 0)?.Value,
                VehicleModel = vehicleDetails?.FirstOrDefault(v => v.Index == 1)?.Value,
                VehicleYear = vehicleDetails?.FirstOrDefault(v => v.Index == 2)?.Value,
                VehicleRegistrationNumber = vehicleDetails?.FirstOrDefault(v => v.Index == 3)?.Value,
                VehicleVIN = vehicleDetails?.FirstOrDefault(v => v.Index == 4)?.Value
            };

            if (!string.IsNullOrWhiteSpace(model.VehicleBrand))
            {
                var make = await _context.CarMakes.FirstOrDefaultAsync(m => m.Name == model.VehicleBrand);
                if (make != null)
                {
                    model.SelectedMakeId = make.Id;

                    if (!string.IsNullOrWhiteSpace(model.VehicleModel))
                    {
                        var carModel = await _context.CarModels
                            .FirstOrDefaultAsync(m => m.CarMakeId == make.Id && m.Name == model.VehicleModel);

                        if (carModel != null)
                            model.SelectedModelId = carModel.Id;
                    }
                }
            }


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(ClientEditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                user.PhoneNumber = model.PhoneNumber;
                var selectedMake = await _context.CarMakes.FindAsync(model.SelectedMakeId);
                var selectedModel = await _context.CarModels.FindAsync(model.SelectedModelId);

                var makeName = selectedMake?.Name ?? model.VehicleBrand;
                var modelName = selectedModel?.Name ?? model.VehicleModel;

                user.VehicleDetails = string.Join(", ",
                    makeName,
                    modelName,
                    model.VehicleYear,
                    model.VehicleRegistrationNumber,
                    model.VehicleVIN
                );


                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Dane zostały zaktualizowane.";
                    return RedirectToAction("Dashboard");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }


        public async Task<IActionResult> VisitHistory()
        {
            var userId = _userManager.GetUserId(User);

            var done = VisitStatus.Done; 

            var raw = await (
                from e in _context.Events
                join w in _context.Users on e.WorkshopId equals w.Id
                where e.ClientId == userId && e.Status == done
                orderby e.Start descending
                select new
                {
                    e.Id,
                    e.Start,
                    e.WorkshopId,
                    WorkshopName = w.CompanyName,
                    w.Street,
                    w.BuildingNumber,
                    w.PostalCode,
                    w.City,
                    ServiceTitle = e.Title,
                    WorkshopNote = e.WorkshopNote,
                    e.VehicleMake,
                    e.VehicleModel,
                    e.VehicleYear,
                    e.VehiclePlate,
                    e.VehicleVin

                }
            ).ToListAsync();

            var visits = raw.Select(x => new ClientVisitHistoryItemVm
            {
                EventId = x.Id,
                Start = x.Start,
                WorkshopId = x.WorkshopId,
                WorkshopName = x.WorkshopName ?? "Nieznany warsztat",
                WorkshopAddress = BuildAddress(x.Street, x.BuildingNumber, x.PostalCode, x.City),
                ServiceTitle = x.ServiceTitle,
                WorkshopNote = x.WorkshopNote,
                VehicleMake = x.VehicleMake,
                VehicleModel = x.VehicleModel,
                VehicleYear = x.VehicleYear == 0 ? null : x.VehicleYear, 
                VehiclePlate = x.VehiclePlate,
                VehicleVin = x.VehicleVin

            }).ToList();

            return View(visits);
        }

        private static string? BuildAddress(string? street, string? building, string? postal, string? city)
        {
            street = street?.Trim();
            building = building?.Trim();
            postal = postal?.Trim();
            city = city?.Trim();

            var line1 = string.Join(" ", new[] { street, building }.Where(s => !string.IsNullOrWhiteSpace(s)));
            var line2 = string.Join(" ", new[] { postal, city }.Where(s => !string.IsNullOrWhiteSpace(s)));

            var full = string.Join(", ", new[] { line1, line2 }.Where(s => !string.IsNullOrWhiteSpace(s)));
            return string.IsNullOrWhiteSpace(full) ? null : full;
        }



        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Hasło zostało zmienione.";
                    return RedirectToAction("Dashboard");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }

            return View();
        }
    }
}
