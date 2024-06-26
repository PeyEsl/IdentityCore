using System.ComponentModel.DataAnnotations;

namespace IdentityCore.Models.DTOs
{
    public class LoginWithPhoneDto
    {
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? Phone { get; set; }
        public bool RememberMe { get; set; }
    }
}
