using IdentityCore.Models;
using Microsoft.AspNetCore.Identity;

public class ApplicationUserLogin : IdentityUserLogin<string>
{
    public virtual ApplicationUser? User { get; set; }
}
