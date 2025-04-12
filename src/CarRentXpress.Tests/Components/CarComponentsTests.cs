using Bunit;
using CarRentXpress.Components.Pages.Car;
using Xunit;
using CarRentXpress.DTOs;    // Assumes CarDto is defined here.

namespace CarRentXpress.Tests.Components
{
    public class CarComponentsTests : TestContext
    {
        [Fact]
        public void AllComponent_RendersProperly()
        {
            var cut = RenderComponent<All>();

            Assert.Contains("All Cars", cut.Markup);
        }

        [Fact]
        public void CreateComponent_RendersForm()
        {
            var cut = RenderComponent<Create>();

            Assert.Contains("<form", cut.Markup, System.StringComparison.OrdinalIgnoreCase);
            Assert.Contains("Create", cut.Markup);
        }

        [Fact]
        public void DetailsComponent_RendersDetails_WhenIdIsProvided()
        {
            var cut = RenderComponent<Details>(parameters => parameters.Add(p => p.Id, "123"));

            Assert.Contains("Car Details", cut.Markup);
            Assert.Contains("123", cut.Markup);
        }

        [Fact]
        public void RentComponent_RendersRentForm()
        {
            var cut = RenderComponent<Rent>();

            Assert.Contains("Rent a Car", cut.Markup);
            Assert.Contains("<form", cut.Markup, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
