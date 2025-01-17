﻿using Microsoft.AspNetCore.Identity;

public class ApplicationRole : IdentityRole
{
    public virtual ICollection<ApplicationUserRole>? UserRoles { get; set; }
    public virtual ICollection<ApplicationRoleClaim>? RoleClaims { get; set; }
}