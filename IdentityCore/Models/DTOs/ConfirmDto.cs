using System.ComponentModel.DataAnnotations;

namespace IdentityCore.Models.DTOs
{
    public class ConfirmDto
    {
        public string? UserId { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }
        public string? Token { get; set; }

        [Required(ErrorMessage = "SMS Code is required.")]
        [StringLength(6)]
        public string? Code { get; set; }
        public string? TokenProvider { get; set; }
        public bool ResetPassword { get; set; }
    }
}
