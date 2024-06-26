using System.ComponentModel.DataAnnotations;

namespace IdentityCore.Models.DTOs
{
    public class RoleDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Role name is required.")]
        public string? Name { get; set; }
    }
}
