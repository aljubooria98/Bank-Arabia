using DataAccessLayer.Models;
using DataAccessLayer.Seeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<BankAppDataContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<BankAppDataContext>();

builder.Services.AddRazorPages();

builder.Services.AddScoped<StatisticsService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<TransactionService>();

builder.Services.AddTransient<DataInitializer>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<BankAppDataContext>();

    var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
    if (pendingMigrations.Any())
    {
        foreach (var migration in pendingMigrations)
            Console.WriteLine(migration);
    }

    await dbContext.Database.MigrateAsync();
    await DataInitializer.SeedDataAsync(services);
    await BankDataSeeder.SeedBankDataAsync(dbContext);
    await DataSeeder.SeedUsersAndRoles(services);
}

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
