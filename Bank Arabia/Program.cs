using DataAccessLayer.Models;
using DataAccessLayer.Seeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Hämta och logga connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
Console.WriteLine("🔌 Använder connection string: " + connectionString);

// Lägg till DbContext med SQL Server
builder.Services.AddDbContext<BankAppDataContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

// Lägg till databasfelsidor för utvecklingsläge
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Konfigurera Identity med roller
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<BankAppDataContext>();

// Razor Pages + tjänster
builder.Services.AddRazorPages();

builder.Services.AddScoped<StatisticsService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<TransactionService>();

builder.Services.AddTransient<DataInitializer>();

var app = builder.Build();

// Migrering + seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<BankAppDataContext>();

    Console.WriteLine("📦 Kollar om databasen är relationell...");
    if (dbContext.Database.IsRelational())
    {
        Console.WriteLine("🚀 Startar migration...");
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            foreach (var migration in pendingMigrations)
                Console.WriteLine($"📝 Migration som körs: {migration}");
        }

        await dbContext.Database.MigrateAsync();
    }

    Console.WriteLine("🌱 Startar seed...");
    await DataInitializer.SeedDataAsync(services);
    await BankDataSeeder.SeedBankDataAsync(dbContext);
    await DataSeeder.SeedUsersAndRoles(services);
    Console.WriteLine("✅ Seed klart!");
}

// Miljöanpassad konfiguration
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
