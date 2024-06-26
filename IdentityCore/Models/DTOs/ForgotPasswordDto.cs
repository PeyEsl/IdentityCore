﻿using System.ComponentModel.DataAnnotations;

namespace IdentityCore.Models.DTOs
{
    public class ForgotPasswordDto
    {
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; set; }
    }
}
