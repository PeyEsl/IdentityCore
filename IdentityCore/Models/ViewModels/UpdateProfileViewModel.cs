﻿using System.ComponentModel.DataAnnotations;

namespace IdentityCore.Models.ViewModels
{
    public class UpdateProfileViewModel
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Required(ErrorMessage = "User name is required.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; set; }
        public IList<string>? Roles { get; set; }
        public IList<string>? RoleList { get; set; }
        public IList<ClaimViewModel>? Claims { get; set; }
    }
}