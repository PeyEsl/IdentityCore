using IdentityCore.Models;
using Microsoft.AspNetCore.Identity;

public class ApplicationUserClaim : IdentityUserClaim<string>
{
    public virtual ApplicationUser? User { get; set; }
}
