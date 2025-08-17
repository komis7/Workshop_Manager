using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkShopManager.Data;
using WorkShopManager.Models;

namespace WorkShopManager.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Dashboard()
        {
            return View(); // Główne menu
        }

        public IActionResult EditProfile()
        {
            var user = _userManager.GetUserAsync(User).Result;

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

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(ClientEditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                user.PhoneNumber = model.PhoneNumber;
                user.VehicleDetails = string.Join(", ", model.VehicleBrand, model.VehicleModel, model.VehicleYear, model.VehicleRegistrationNumber, model.VehicleVIN);

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

        public IActionResult VisitHistory()
        {
            var visits = new List<VisitViewModel>
            {
                new VisitViewModel { Date = DateTime.Now, WorkshopName = "Warsztat 1", Service = "Wymiana opon" },
                new VisitViewModel { Date = DateTime.Now.AddDays(-5), WorkshopName = "Warsztat 2", Service = "Przegląd techniczny" }
            };
            return View(visits);
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
                return RedirectToAction("Index"); // lub inna strona po zalogowaniu
            }

            return View();
        }
    }
}
