using BicycleAPI.Domain.Entities.Permission;
using BicycleAPI.Domain.Entities.RefreshToken;
using BicycleAPI.Domain.Entities.Role;
using BicycleAPI.Domain.Entities.RolePermission;
using BicycleAPI.Domain.Entities.User;
using BicycleAPI.Domain.Entities.UserRole;
using Microsoft.EntityFrameworkCore;

namespace BicycleAPI.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<PermissionEntity> Permissions { get; set; }
    public DbSet<UserRoleEntity> UserRoles { get; set; }
    public DbSet<RolePermissionEntity> RolePermissions { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 自動套用所有 Configuration
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // 讓所有 DateTime 都自動對應到 timestamptz
        configurationBuilder.Properties<DateTime>().HaveColumnType("timestamp with time zone");
    }
}
