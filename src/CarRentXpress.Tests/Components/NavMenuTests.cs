using System.Security.Claims;
using Bunit;
using Bunit.TestDoubles;
using CarRentXpress.Components.Layout;
using Xunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using CarRentXpress;  

namespace CarRentXpress.Tests.Components
{
    public class NavMenuTests : TestContext
    {
        [Fact]
        public void NavMenu_RendersAdminLinks_WhenUserIsAdmin()
        {
            var authContext = this.AddTestAuthorization();
            authContext.SetAuthorized("AdminUser");
            authContext.SetRoles("Admin");

            var cut = RenderComponent<NavMenu>();

            Assert.Contains("Create Car", cut.Markup);
            Assert.Contains("AdminUser", cut.Markup);
            Assert.Contains("Logout", cut.Markup);
        }

        [Fact]
        public void NavMenu_DoesNotRenderAdminLinks_WhenUserIsNotAdmin()
        {
            var authContext = this.AddTestAuthorization();
            authContext.SetAuthorized("RegularUser");
            authContext.SetRoles("User"); 

            var cut = RenderComponent<NavMenu>();

            Assert.DoesNotContain("Create Car", cut.Markup);
            Assert.Contains("RegularUser", cut.Markup);
            Assert.Contains("Logout", cut.Markup);
        }

        [Fact]
        public void NavMenu_RendersNotAuthorizedLinks_WhenUserNotAuthorized()
        {
            var authContext = this.AddTestAuthorization();
            authContext.SetNotAuthorized();

            var cut = RenderComponent<NavMenu>();

            Assert.Contains("Register", cut.Markup);
            Assert.Contains("Login", cut.Markup);
            Assert.DoesNotContain("Logout", cut.Markup);
        }
    }
}
