using BicycleAPI.Domain.Entities.Role;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BicycleAPI.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Name)
            .HasMaxLength(100);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        // 唯一索引
        builder.HasIndex(r => r.Code).IsUnique();
        builder.HasIndex(r => r.Name).IsUnique();
    }
}
