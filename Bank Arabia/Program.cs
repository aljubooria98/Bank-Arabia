using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DataAccessLayer.Data;
using DataAccessLayer.Seeds;
using Services.Services;

var builder = WebApplication.CreateBuilder(args);

// 🔌 Anslutningssträng
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 📦 Registrera DbContexts
builder.Services.AddDbContext<BankAppDataContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 👥 Identity + roller
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// 🧩 Tjänster
builder.Services.AddScoped<StatisticsService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<TransactionService>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddRazorPages();
builder.Services.AddControllers(); // API-stöd

var app = builder.Build();

// 🚀 Automatiska migreringar och seeders
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var identityDb = services.GetRequiredService<ApplicationDbContext>();
        var bankDb = services.GetRequiredService<BankAppDataContext>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        // 🛠 Kör migreringar om databasen inte redan är uppdaterad
        try
        {
            await identityDb.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "⚠️ IdentityDbContext: Troligen finns tabellerna redan.");
        }

        try
        {
            await bankDb.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "⚠️ BankAppDataContext: Troligen finns tabellerna redan.");
        }

        // 🌱 Seed endast om data saknas
        if (!await identityDb.Roles.AnyAsync())
        {
            logger.LogInformation("🌱 Seedar användare och roller...");
            await DataSeeder.SeedUsersAndRoles(services);
        }

        if (!await bankDb.Customers.AnyAsync())
        {
            logger.LogInformation("🌱 Seedar bankdata...");
            await BankDataSeeder.SeedBankDataAsync(bankDb);
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ Allvarligt fel vid migrering/seed.");
        throw;
    }
}

// 🌐 Middleware
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllers(); // API-endpoints

app.Run();
