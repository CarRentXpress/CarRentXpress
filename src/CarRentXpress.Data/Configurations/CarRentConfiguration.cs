using CarRentXpress.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRentXpress.Data.Configurations;

public class CarRentConfiguration : IEntityTypeConfiguration<CarRent>
{
    void IEntityTypeConfiguration<CarRent>.Configure(EntityTypeBuilder<CarRent> builder)
    {
        builder.HasOne(r => r.Car)
            .WithMany() // Assuming a Car can have many rentals, if not use .WithOne() instead
            .IsRequired();

        // Configure the ApplicationUser foreign key relationship
        builder.HasOne(r => r.User)
            .WithMany() // Assuming a User can have many rentals, if not use .WithOne() instead
            .IsRequired();
    }
}