using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IdentityCore.Models;
using IdentityCore.Tools;
using IdentityCore.Models.ViewModels;

namespace IdentityCore.Data
{
    public class ApplicationDbContext : IdentityDbContext<
        ApplicationUser, ApplicationRole, string,
        ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin,
        ApplicationRoleClaim, ApplicationUserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<EmailSetting>().HasData(new EmailSetting()
            {
                Id = 1,
                DisplayName = "IdentityCore Email",
                From = "YOUR_EMAIL_ADDRESS",
                Password = "YOUR_APP_PASSWORD", // Enabling SMTP in Gmail settings
                SMTP = "smtp.gmail.com",
                Port = 587,
                EnableSSL = true,
                IsDefault = true,
                IsDelete = false,
                CreateDate = DateTime.Now,
            });

            builder.Entity<ApplicationUser>(u =>
            {
                u.ToTable("Users");

                // Each User can have many UserClaims
                u.HasMany(u => u.Claims)
                    .WithOne(c => c.User)
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                // Each User can have many UserLogins
                u.HasMany(u => u.Logins)
                    .WithOne(ul => ul.User)
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // Each User can have many UserTokens
                u.HasMany(u => u.Tokens)
                    .WithOne(ut => ut.User)
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                // Each User can have many entries in the UserRole join table
                u.HasMany(u => u.UserRoles)
                    .WithOne(ur => ur.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<ApplicationRole>(r =>
            {
                r.ToTable("Roles");

                // Each Role can have many entries in the UserRole join table
                r.HasMany(r => r.UserRoles)
                    .WithOne(ur => ur.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                // Each Role can have many associated RoleClaims
                r.HasMany(r => r.RoleClaims)
                    .WithOne(rc => rc.Role)
                    .HasForeignKey(rc => rc.RoleId)
                    .IsRequired();
            });

            builder.Entity<ApplicationUserRole>(ur =>
            {
                ur.ToTable("UserRoles");
            });

            builder.Entity<ApplicationRoleClaim>(rc =>
            {
                rc.ToTable("RoleClaims");
            });

            builder.Entity<ApplicationUserClaim>(uc =>
            {
                uc.ToTable("UserClaims");
            });

            builder.Entity<ApplicationUserLogin>(ul =>
            {
                ul.ToTable("UserLogins");
            });

            builder.Entity<ApplicationUserToken>(ut =>
            {
                ut.ToTable("UserTokens");
            });
        }

        public DbSet<EmailSetting> EmailSettings { get; set; } = default!;
        public DbSet<IdentityCore.Models.ViewModels.ConfirmViewModel> ConfirmViewModel { get; set; } = default!;
    }
}
