using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IdentityCore.Models.ViewModels
{
    public class UserViewModel
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Required(ErrorMessage = "User name is required.")]
        [Remote("IsAnyUserName", "Account", HttpMethod = "Post", AdditionalFields = "__RequestVerificationToken")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Remote("IsAnyEmail", "Account", HttpMethod = "Post", AdditionalFields = "__RequestVerificationToken")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [Remote("IsAnyPhone", "Account", HttpMethod = "Post", AdditionalFields = "__RequestVerificationToken")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }
        public bool AcceptPolicy { get; set; }
        public bool SendLinkByEmail { get; set; }
        public IList<string>? Roles { get; set; }
        public IList<ClaimViewModel>? Claims { get; set; }
    }
}
