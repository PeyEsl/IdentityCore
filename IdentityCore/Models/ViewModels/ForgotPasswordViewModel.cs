using System.ComponentModel.DataAnnotations;

namespace IdentityCore.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        public string? Id { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; set; }
        public bool SendResetLinkByEmail { get; set; }
    }
}
