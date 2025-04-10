using System.Reflection;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using CarRentXpress.Client.Pages;
using CarRentXpress.Components;
using CarRentXpress.Components.Account;
using CarRentXpress.Application.Services;
using CarRentXpress.Application.Services.Contracts;
using CarRentXpress.Data;
using CarRentXpress.Profiles;
using CarRentXpress.Application.Scraping;
using CarRentXpress.Core.Repositories;
using CarRentXpress.Data.Entities;
using CarRentXpress.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<CarScraperService>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

Assembly currentAssembly = Assembly.GetExecutingAssembly();
builder.Services.AddAutoMapper(currentAssembly);
builder.Services.AddAutoMapper(typeof(CarProfile));

var app = builder.Build();

// When "scrape" argument is passed, create a scope and resolve the scraper service from that scope.
if (args.Contains("scrape"))
{
    using (var scope = app.Services.CreateScope())
    {
        var scraperService = scope.ServiceProvider.GetRequiredService<CarScraperService>();
        await scraperService.ScrapeAndPersistCarsAsync();
    }
    return; 
}

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(CarRentXpress.Client._Imports).Assembly);
app.MapAdditionalIdentityEndpoints();

app.Run();
