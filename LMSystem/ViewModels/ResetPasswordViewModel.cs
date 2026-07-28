using System.ComponentModel.DataAnnotations;

namespace LMSystem.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Token { get; set; }

        [Required(ErrorMessage = "Please choose a new password.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Password must be at least 3 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Please confirm your new password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
