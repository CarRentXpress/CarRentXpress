using CarRentXpress.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRentXpress.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName)
            .HasDefaultValue("DefaultFirstName")
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasDefaultValue("DefaultLastName")
            .IsRequired();

        builder.Property(u => u.EGN)
            .IsRequired();
        
        builder.HasIndex(u => u.EGN)
            .IsUnique();
    }
}