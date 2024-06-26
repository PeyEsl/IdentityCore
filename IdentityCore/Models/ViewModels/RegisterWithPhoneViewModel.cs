using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IdentityCore.Models.ViewModels
{
    public class RegisterWithPhoneViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [Remote("IsAnyPhone", "Account", HttpMethod = "Post", AdditionalFields = "__RequestVerificationToken")]
        public string? PhoneNumber { get; set; }
        public bool AcceptPolicy { get; set; }
    }
}
