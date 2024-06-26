using System.ComponentModel.DataAnnotations;

namespace IdentityCore.Models.ViewModels
{
    public class LoginWithPhoneViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? Phone { get; set; }
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
