using BicycleAPI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BicycleAPI.Api.Extensions;

public static class ApplicationExtension
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            // 只有在 "開發環境" 才自動執行 Migrate
            if (app.Environment.IsDevelopment())
            {
                logger.LogInformation("正在執行資料庫遷移 (Development)...");
                await context.Database.MigrateAsync();
            }
            else
            {
                logger.LogInformation("正式環境跳過自動 Migration，請確認 Schema 已透過 CI/CD 更新。");
            }

            // 不分環境，都嘗試補齊權限
            logger.LogInformation("正在同步權限資料...");

            var seeder = services.GetRequiredService<PermissionSeeder>();
            await seeder.SyncPermissionsAsync();

            logger.LogInformation("資料庫初始化完成。");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "應用程式啟動時，資料庫初始化發生錯誤。");

            throw new InvalidOperationException("資料庫初始化失敗,請檢查資料庫連線設定與遷移狀態。", ex);
        }
    }
}
