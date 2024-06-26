using IdentityCore.Models;
using Microsoft.AspNetCore.Identity;

public class ApplicationUserToken : IdentityUserToken<string>
{
    public virtual ApplicationUser? User { get; set; }
}