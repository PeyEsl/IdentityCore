using System.ComponentModel.DataAnnotations;

namespace IdentityCore.Models.DTOs
{
    public class ClaimDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Claim type is required.")]
        public string? Type { get; set; }

        [Required(ErrorMessage = "Claim value is required.")]
        public string? Value { get; set; }
    }
}
