using System.ComponentModel.DataAnnotations;

namespace WorkShopManager.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Podaj obecne hasło.")]
        [DataType(DataType.Password)]
        [Display(Name = "Obecne hasło")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Podaj nowe hasło.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nowe hasło")]
        [StringLength(100, ErrorMessage = "Hasło musi mieć co najmniej {2} znaków.", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Potwierdź nowe hasło.")]
        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("NewPassword", ErrorMessage = "Nowe hasło i potwierdzenie hasła muszą być takie same.")]
        public string ConfirmPassword { get; set; }
    }
}

