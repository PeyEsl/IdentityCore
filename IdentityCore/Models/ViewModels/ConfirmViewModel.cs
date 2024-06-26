using System.ComponentModel.DataAnnotations;

namespace IdentityCore.Models.ViewModels
{
    public class ConfirmViewModel
    {
        public string? Id { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }
        public string? Token { get; set; }

        [Required(ErrorMessage = "SMS Code is required.")]
        [StringLength(6)]
        public string? Code { get; set; }

        [StringLength(6)]
        public string? SendCode { get; set; }
        public bool ResetPassword { get; set; }
        public string? CallBackUrl { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
