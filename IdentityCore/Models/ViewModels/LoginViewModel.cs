using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace IdentityCore.Models.ViewModels
{
    public class LoginViewModel
    {
        public string? Id { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
