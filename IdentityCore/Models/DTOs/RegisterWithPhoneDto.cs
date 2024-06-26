using System.ComponentModel.DataAnnotations;

namespace IdentityCore.Models.DTOs
{
    public class RegisterWithPhoneDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; set; }
        public bool AcceptPolicy { get; set; }
    }
}
