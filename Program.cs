using AirportWebApp.Data;
using AirportWebApp.Models;
using AirportWebApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<AirportDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddErrorDescriber<AirportWebApp.Services.SpanishIdentityErrorDescriber>()
    .AddRoles<Microsoft.AspNetCore.Identity.IdentityRole>()
    .AddEntityFrameworkStores<AirportDbContext>();

builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, AirportWebApp.Services.SmtpEmailSender>();

builder.Services.AddScoped(sp =>
{
    var config = builder.Configuration;
    var clientId = config["PayPal:ClientId"] ?? throw new InvalidOperationException("PayPal:ClientId not set");
    var secret = config["PayPal:ClientSecret"] ?? throw new InvalidOperationException("PayPal:ClientSecret not set");
    var mode = config["PayPal:Mode"] ?? "sandbox";
    return new PayPalService(clientId, secret, mode);
});

var app = builder.Build();

// Ensure roles exist and optionally create an admin user from secrets
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();
        var userManager = services.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>>();
        var config = services.GetRequiredService<IConfiguration>();

        string[] roles = new[] { "Administrador", "Cliente" };
        foreach (var role in roles)
        {
            var exists = roleManager.RoleExistsAsync(role).GetAwaiter().GetResult();
            if (!exists)
            {
                roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(role)).GetAwaiter().GetResult();
            }
        }

        var adminEmail = "admin@airportwebapp.local";
        var adminPassword = "Admin123!";
        var admin = userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult();
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            var createRes = userManager.CreateAsync(admin, adminPassword).GetAwaiter().GetResult();
            if (createRes.Succeeded)
            {
                userManager.AddToRoleAsync(admin, "Administrador").GetAwaiter().GetResult();
            }
        }
    }
    catch
    {
        // ignore errors during startup role seeding
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
