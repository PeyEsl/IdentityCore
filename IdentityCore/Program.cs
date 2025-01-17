using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityCore.Services;
using IdentityCore.Models;
using IdentityCore.Tools;
using IdentityCore.Data;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Identity services
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddUserManager<UserManager<ApplicationUser>>()
    .AddRoles<ApplicationRole>()
    .AddRoleManager<RoleManager<ApplicationRole>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<PersianIdentityErrors>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // User settings.
    options.User.RequireUniqueEmail = true;

    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = true;

    // SignIn settings.
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;

    // Lockout settings.
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

    options.Stores.ProtectPersonalData = false;
});

// Add application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISiteSettingRepository, SiteSettingRepository>();
builder.Services.AddScoped<IViewRenderService, ViewRenderService>();

builder.Services.AddControllersWithViews();

// Configure application cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(1);

    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/LoginFailed";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
