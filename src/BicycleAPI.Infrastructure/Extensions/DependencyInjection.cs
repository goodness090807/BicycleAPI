using System.Reflection;
using BicycleAPI.Domain.Repositories;
using BicycleAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Security.PasswordHasher;

namespace BicycleAPI.Infrastructure.Extensions;

public static class DependencyInjection
{
    /// <summary>
    /// 註冊 Infrastructure 層的服務，包含 DbContext、Repositories 等
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 註冊 Interceptor 為 Scoped
        services.AddScoped<AuditingInterceptor>();

        // 註冊 DbContext
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(connectionString).AddInterceptors(sp.GetRequiredService<AuditingInterceptor>());
        });

        // 自動註冊所有 Repository
        RegisterRepositories(services);

        // 註冊 Password Hasher
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

        return services;
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        // 取得 Infrastructure 的 Assembly
        var assembly = Assembly.GetExecutingAssembly();

        // 搜尋符合條件的類別
        var concreteTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsSealed);

        foreach (var type in concreteTypes)
        {
            var interfaces = type.GetInterfaces();

            var repositoryInterface = interfaces.FirstOrDefault(i =>
                !i.IsGenericType || i.GetGenericTypeDefinition() != typeof(IGenericRepository<>)
                && i.GetInterfaces().Any(parent =>
                    parent.IsGenericType &&
                    parent.GetGenericTypeDefinition() == typeof(IGenericRepository<>)));

            if (repositoryInterface != null)
            {
                // 註冊介面與實作，使用 Scoped 生命週期
                services.AddScoped(repositoryInterface, type);
            }
        }
    }
}
