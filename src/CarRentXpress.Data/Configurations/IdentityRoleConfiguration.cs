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
            var roles = new List<string>
            {
                Role.User,
                Role.Admin
            };

            var identityRoles = roles
                .Select(role => new IdentityRole
                {
                    Id = role, // or use role if you want it to be predictable
                    Name = role,
                    NormalizedName = role.ToUpper()
                })
                .ToList();

            builder.HasData(identityRoles);
        }
    }
}