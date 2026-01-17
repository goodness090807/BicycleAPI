using BicycleAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BicycleAPI.Infrastructure.Persistence.Configurations;

public abstract class BaseAuditableEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseAuditableEntity
{
       public virtual void Configure(EntityTypeBuilder<TEntity> builder)
       {
              builder.Property(x => x.CreatedAt)
                     .IsRequired()
                     .HasDefaultValueSql("CURRENT_TIMESTAMP");

              builder.Property(x => x.CreatedBy)
                     .HasMaxLength(50)
                     .IsRequired(false);

              builder.Property(x => x.UpdatedAt)
                     .IsRequired(false);

              builder.Property(x => x.UpdatedBy)
                     .HasMaxLength(50)
                     .IsRequired(false);
       }
}
