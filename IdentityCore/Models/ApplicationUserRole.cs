using IdentityCore.Models;
using Microsoft.AspNetCore.Identity;

public class ApplicationUserRole : IdentityUserRole<string>
{
    public virtual ApplicationUser? User { get; set; }
    public virtual ApplicationRole? Role { get; set; }
}