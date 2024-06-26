using System.ComponentModel.DataAnnotations;

namespace IdentityCore.Models.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "User name is required.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
