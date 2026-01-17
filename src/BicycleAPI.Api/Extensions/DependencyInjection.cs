using System.Reflection;
using BicycleAPI.Infrastructure.Persistence;
using FluentValidation;
using Shared.Security.JWTHandler;

namespace BicycleAPI.Api.Extensions;

public static class DependencyInjection
{
    /// <summary>
    /// 註冊 Application 層的服務
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // 取得 Application 層的 Assembly（透過任一 Application 層的型別）
        var applicationAssembly = Assembly.Load("BicycleAPI.Application");

        // 註冊 HttpContextAccessor，用於取得目前使用者資訊
        services.AddHttpContextAccessor();

        services.AddScoped<PermissionSeeder>();

        // 註冊 FluentValidation Validators
        services.AddValidatorsFromAssembly(applicationAssembly);

        return services;
    }

    public static IServiceCollection AddShared(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TokenHandlerSettings>(configuration.GetSection("JWT"));
        services.AddTransient<ITokenHandler, TokenHandler>();

        return services;
    }
}
