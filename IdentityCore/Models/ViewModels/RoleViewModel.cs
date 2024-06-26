using System.ComponentModel.DataAnnotations;

namespace IdentityCore.Models.ViewModels
{
    public class RoleViewModel
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Role name is required.")]
        public string? Name { get; set; }
        public IList<ClaimViewModel>? Claims { get; set; }
    }
}
