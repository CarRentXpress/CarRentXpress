using System.Reflection;
using CarRentXpress.Application.Scraping;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using CarRentXpress.Components;
using CarRentXpress.Components.Account;
using CarRentXpress.Core.Repositories;
using CarRentXpress.Data;
using CarRentXpress.Data.Entities;
using CarRentXpress.Data.Repositories;
using CarRentXpress.Profiles;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddUserManager<UserManager<ApplicationUser>>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IFileUploadService, FirebaseStorageService>();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddScoped<CarRentXpress.Application.Services.Interfaces.ICarService, CarRentXpress.Application.Services.CarService>();
builder.Services.AddScoped<CarScraperService>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

Assembly currentAssembly = Assembly.GetExecutingAssembly();
builder.Services.AddAutoMapper(currentAssembly);
builder.Services.AddAutoMapper(typeof(CarProfile));

var app = builder.Build();

if (args.Contains("scrape"))
{
    using (var scope = app.Services.CreateScope())
    {
        var scraperService = scope.ServiceProvider.GetRequiredService<CarScraperService>();
        await scraperService.ScrapeAndPersistCarsAsync();
    }
    return; 
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
