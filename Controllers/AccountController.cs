using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkShopManager.Models;

namespace CarWorkshopAppointments.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    VehicleDetails = model.VehicleDetails
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        Console.WriteLine($"Rola użytkownika: {string.Join(", ", roles)}");

                        if (roles.Contains("Client"))
                        {
                            return RedirectToAction("Dashboard", "Client");
                        }
                        if (roles.Contains("Workshop"))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Nieprawidłowy login lub hasło");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ChooseRegistrationType()
        {
            return View();
        }

        public IActionResult RegisterClient()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterClient(RegisterClientViewModel model)
            {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    VehicleDetails = $"{model.VehicleBrand}, {model.VehicleModel},{model.VehicleYear}, {model.VehicleRegistrationNumber}, {model.VehicleVIN}"
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Przypisanie roli "Client" nowo zarejestrowanemu użytkownikowi
                    await _userManager.AddToRoleAsync(user, "Client");

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Dashboard", "Client");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        public IActionResult RegisterWorkshop()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterWorkshop(RegisterWorkshopViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    CompanyName = model.CompanyName,
                    Street = model.Street,
                    BuildingNumber = model.BuildingNumber.ToString(),
                    PostalCode = model.PostalCode,
                    City = model.City,
                    Services = string.Join(", ", model.SelectedServices),
                    HourlyRate = model.HourlyRate
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Workshop");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> UserPanel()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Workshop"))
            {
                return RedirectToAction("Dashboard", "Workshop");
            }
            else if (roles.Contains("Client"))
            {
                return RedirectToAction("Dashboard", "Client");
            }

            // Domyślne przekierowanie (jeśli rola nie jest rozpoznana)
            return RedirectToAction("Index", "Home");
        }

    }

}

