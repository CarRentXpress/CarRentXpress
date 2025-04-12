using System.Reflection;
using System.Security.Claims;
using Bunit;
using Bunit.TestDoubles;
using CarRentXpress.Application.Scraping;
using CarRentXpress.Application.Services;
using CarRentXpress.Components.Pages.Car;
using CarRentXpress.Core.Enums;
using CarRentXpress.Core.Repositories;
using CarRentXpress.Data;
using CarRentXpress.Data.Entities;
using CarRentXpress.Data.Repositories;
using Xunit;
using CarRentXpress.DTOs;
using CarRentXpress.Profiles;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MudBlazor;

namespace CarRentXpress.Tests.Components
{
    public class CarComponentsTests : TestContext
    {
        private TestAuthorizationContext _authContext;
        public CarComponentsTests()
        {
            // Register services needed for the tests
            Services.AddMudServices();
            Services.AddMudBlazorSnackbar();
            Services.AddMudPopoverService();
            Services.AddSingleton<MudPopoverProvider>();
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                // Add other claims if needed
            }));

            // Register the mock HttpContextAccessor
            Services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor { HttpContext = mockHttpContext });
        
            // Register DbContext with InMemory Database for testing
            Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase"));

            // Register Identity services with ApplicationDbContext
            Services.AddIdentity<ApplicationUser, IdentityRole>(options => 
                {
                    
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Registering other services for the tests
            Services.AddScoped<IUserStore<ApplicationUser>, UserStore<ApplicationUser, IdentityRole, ApplicationDbContext>>();
            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
        
            // Mock IOptions<IdentityOptions>
            var mockOptions = new Mock<IOptions<IdentityOptions>>();

            // Mock IUserManager
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                mockUserStore.Object,
                mockOptions.Object,
                Mock.Of<IPasswordHasher<ApplicationUser>>(),
                new List<IUserValidator<ApplicationUser>>(),
                new List<IPasswordValidator<ApplicationUser>>(),
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<ApplicationUser>>>()
            );

            var mockUser = new ApplicationUser { UserName = "testuser" };

            // Mock GetUserAsync to return the mock user
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(mockUser);

            // Mock GetRolesAsync to return a list of roles
            mockUserManager.Setup(um => um.GetRolesAsync(mockUser)).ReturnsAsync(new List<string> { Role.Admin });


            // Register mocked services in DI container
            Services.AddSingleton(mockUserManager.Object);
            Services.AddScoped<SignInManager<ApplicationUser>>();

            // Registering application services
            Services.AddScoped<IFileUploadService, FirebaseStorageService>();
            Services.AddScoped<ICarRentService, CarRentService>();
            Services.AddScoped<CarScraperService>();
            Services.AddScoped<CarRentXpress.Application.Services.Interfaces.ICarService, CarRentXpress.Application.Services.CarService>();
        
            // Add AutoMapper configurations
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            Services.AddAutoMapper(currentAssembly);
            Services.AddAutoMapper(typeof(CarProfile));
            Services.AddAutoMapper(typeof(CarRentProfile));

            // Add repository
            Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Add test authorization context
            _authContext = this.AddTestAuthorization();
            JSInterop.SetupVoid("mudPopover.initialize", "mudblazor-main-content", 0);
            JSInterop.SetupVoid("mudElementRef.addOnBlurEvent", _ => true);
            JSInterop.SetupVoid("mudKeyInterceptor.connect", _ => true);
        }

        [Fact]
        public async Task AdminCanAccessCreateCar()
        {
            // Arrange
            _authContext.SetAuthorized("testuser");
            _authContext.SetRoles(Role.Admin);
	
            // Act
            var cut = RenderComponent<CarRentXpress.Components.Pages.Car.Create>();

            // Assert
            Assert.NotEqual("", cut.Markup);

            Assert.Contains("<form", cut.Markup);
        }

        [Fact]
        public async Task UserCantAccessCreateCar()
        {
            // Arrange
            _authContext.SetAuthorized("testuser");
            _authContext.SetRoles(Role.User);
	
            // Act
            var cut = RenderComponent<CarRentXpress.Components.Pages.Car.Create>();

            // Assert
            Assert.Equal("", cut.Markup);
        }
        
        [Fact]
        public async Task UnauthedCantAccessCreateCar()
        {
            // Arrange
	
            // Act
            var cut = RenderComponent<CarRentXpress.Components.Pages.Car.Create>();

            // Assert
            Assert.Equal("", cut.Markup);
        }
        
        [Fact]
        public async Task AdminCanAccessAllCars()
        {
            // Arrange
            _authContext.SetAuthorized("testuser");
            _authContext.SetRoles(Role.Admin);
	
            // Act
            var cut = RenderComponent<CarRentXpress.Components.Pages.Car.All>();

            // Assert
            Assert.NotEqual("", cut.Markup);
        }
        
        [Fact]
        public async Task UserCanAccessAllCars()
        {
            // Arrange
            _authContext.SetAuthorized("testuser");
            _authContext.SetRoles(Role.User);
	
            // Act
            var cut = RenderComponent<CarRentXpress.Components.Pages.Car.All>();

            // Assert
            Assert.NotEqual("", cut.Markup);
        }
        
        [Fact]
        public async Task UnauthedCanAccessAllCars()
        {
            // Arrange
	
            // Act
            var cut = RenderComponent<CarRentXpress.Components.Pages.Car.All>();

            // Assert
            Assert.NotEqual("", cut.Markup);
        }
        
        [Fact]
        public async Task AdminCanAccessEditCar()
        {
            // Arrange
            _authContext.SetAuthorized("testuser");
            _authContext.SetRoles(Role.Admin);
	
            // Act
            var cut = RenderComponent<CarRentXpress.Components.Pages.Car.Edit>();

            // Assert
            Assert.NotEqual("", cut.Markup);
        }
        
        [Fact]
        public async Task UserCantAccessEditCar()
        {
            // Arrange
            _authContext.SetAuthorized("testuser");
            _authContext.SetRoles(Role.User);
	
            // Act
            var cut = RenderComponent<CarRentXpress.Components.Pages.Car.Edit>();

            // Assert
            Assert.Equal("", cut.Markup);
        }
        
        [Fact]
        public async Task UnauthedCantAccessEditCar()
        {
            // Arrange
            _authContext.SetAuthorized("testuser");
            _authContext.SetRoles(Role.User);
	
            // Act
            var cut = RenderComponent<CarRentXpress.Components.Pages.Car.Edit>();

            // Assert
            Assert.Equal("", cut.Markup);
        }
        
        [Fact]
        public async Task AdminCanAccessRentCar()
        {
            // Arrange
            _authContext.SetAuthorized("testuser");
            _authContext.SetRoles(Role.Admin);
	
            // Act
            var cut = RenderComponent<CarRentXpress.Components.Pages.Car.Rent>();

            // Assert
            Assert.NotEqual("", cut.Markup);
        }
        
        [Fact]
        public async Task UserCanAccessRentCar()
        {
            // Arrange
            _authContext.SetAuthorized("testuser");
            _authContext.SetRoles(Role.User);
	
            // Act
            var cut = RenderComponent<CarRentXpress.Components.Pages.Car.Rent>();

            // Assert
            Assert.NotEqual("", cut.Markup);
        }
        
        [Fact]
        public async Task UnauthedCantAccessRentCar()
        {
            // Arrange
            
            // Act
            var cut = RenderComponent<CarRentXpress.Components.Pages.Car.Rent>();

            // Assert
            Assert.Equal("", cut.Markup);
        }

        [Fact] public async Task AdminCanAccessCarDetails()
        {
            // Arrange
            _authContext.SetAuthorized("testuser");
            _authContext.SetRoles(Role.Admin);
	
            // Act
            var cut = RenderComponent<CarRentXpress.Components.Pages.Car.Details>();

            // Assert
            Assert.NotEqual("", cut.Markup);
        }

        [Fact] public async Task UserCanAccessCarDetails()
        {
            // Arrange
            _authContext.SetAuthorized("testuser");
            _authContext.SetRoles(Role.User);
	
            // Act
            var cut = RenderComponent<CarRentXpress.Components.Pages.Car.Details>();

            // Assert
            Assert.NotEqual("", cut.Markup);
        }

        [Fact] 
        public async Task UnauthedCanAccessCarDetails()
        {
            // Arrange
	
            // Act
            var cut = RenderComponent<CarRentXpress.Components.Pages.Car.Details>();

            // Assert
            Assert.NotEqual("", cut.Markup);
        }

        [Fact] public async Task AdminCanAccessAllCarRents()
        {
            // Arrange
            _authContext.SetAuthorized("testuser");
            _authContext.SetRoles(Role.Admin);
	
            // Act
            var cut = RenderComponent<CarRentXpress.Components.Pages.CarRent.All>();

            // Assert
            Assert.NotEqual("", cut.Markup);
        }

        [Fact] public async Task UserCanAccessAllCarRents()
        {
            // Arrange
            _authContext.SetAuthorized("testuser");
            _authContext.SetRoles(Role.User);
	
            // Act
            var cut = RenderComponent<CarRentXpress.Components.Pages.CarRent.All>();

            // Assert
            Assert.NotEqual("", cut.Markup);
        }
        
        [Fact] public async Task UnauthedCantAccessAllCarRents()
        {
            // Arrange
	
            // Act
            var cut = RenderComponent<CarRentXpress.Components.Pages.CarRent.All>();

            // Assert
            Assert.Equal("", cut.Markup);
        }
    }
}
