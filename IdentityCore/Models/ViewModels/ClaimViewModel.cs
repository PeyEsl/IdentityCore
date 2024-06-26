using System.ComponentModel.DataAnnotations;

namespace IdentityCore.Models.ViewModels
{
    public class ClaimViewModel
    {
        public string? Id { get; set; }
        public string? RoleName { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? ClaimType { get; set; }

        [Required(ErrorMessage = "Claim type is required.")]
        public string? Type { get; set; }

        [Required(ErrorMessage = "Claim value is required.")]
        public string? Value { get; set; }
    }
}
