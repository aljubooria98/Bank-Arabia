using DataAccessLayer.Models;
using DataAccessLayer.Seeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Services;

var builder = WebApplication.CreateBuilder(args);

// 📡 Connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
Console.WriteLine("🔌 Använder connection string: " + connectionString);

// 🔧 Konfigurera DbContext med retry-policy
builder.Services.AddDbContext<BankAppDataContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

// 💥 Utvecklingsverktyg för migrationsfel
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 🔐 Identity-konfiguration
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<BankAppDataContext>();

// 📄 Razor + tjänster
builder.Services.AddRazorPages();

builder.Services.AddScoped<StatisticsService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<TransactionService>();

builder.Services.AddTransient<DataInitializer>();

var app = builder.Build();

// 🚀 Automatisk migrering & seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<BankAppDataContext>();

    try
    {
        Console.WriteLine("📦 Kontrollerar databas...");
        if (dbContext.Database.IsRelational())
        {
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                Console.WriteLine("🛠️ Migreringar som körs:");
                foreach (var migration in pendingMigrations)
                    Console.WriteLine($" - {migration}");

                await dbContext.Database.MigrateAsync();
            }
            else
            {
                Console.WriteLine("✅ Inga nya migreringar behövs.");
            }
        }

        // 🌱 Seed Data – Idempotent (görs bara om det behövs)
        Console.WriteLine("🌱 Kör SeedData...");
        await DataInitializer.SeedDataAsync(services);
        await BankDataSeeder.SeedBankDataAsync(dbContext);
        await DataSeeder.SeedUsersAndRoles(services);
        Console.WriteLine("✅ Seed klart!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"🚨 Fel under migration/seed: {ex.Message}");
    }
}

// 🌍 Middleware & routing
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

app.Run();
