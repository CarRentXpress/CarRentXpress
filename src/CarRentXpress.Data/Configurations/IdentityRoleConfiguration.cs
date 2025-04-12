using CarRentXpress.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRentXpress.Data.Configurations
{
    public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            var roles = Enum.GetValues(typeof(Role))
                .Cast<Role>()
                .Select(role => new IdentityRole
                {
                    Id = role.ToString(),
                    Name = role.ToString(),
                    NormalizedName = role.ToString().ToUpper()
                })
                .ToList();

            builder.HasData(roles);
        }
    }
}