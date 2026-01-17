using System.ComponentModel;
using System.Reflection;
using BicycleAPI.Domain.Entities.Permission;
using BicycleAPI.Domain.Variables.Permissions;
using Microsoft.EntityFrameworkCore;

namespace BicycleAPI.Infrastructure.Persistence;

public class PermissionSeeder
{
    public record PermissionInfo(string Code, string Name, string Group);

    private readonly ApplicationDbContext _context;

    public PermissionSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SyncPermissionsAsync()
    {
        var permissionInfos = GetPermissionInfos();

        var existingPermissions = await _context.Permissions.AsNoTracking().Select(p => p.Code).ToListAsync();

        var newPermissions = permissionInfos
            .Where(p => !existingPermissions.Contains(p.Code))
            .Select(p => PermissionEntity.Create(p.Code, p.Name, p.Group))
            .Where(result => result.IsSuccess)
            .Select(result => result.Value)
            .ToList();

        // 4. 批量寫入資料庫
        if (newPermissions.Any())
        {
            await _context.Permissions.AddRangeAsync(newPermissions);
            await _context.SaveChangesAsync();
        }

        await _context.SaveChangesAsync();
    }

    private static IEnumerable<PermissionInfo> GetPermissionInfos()
    {
        var nestedTypes = typeof(Permissions).GetNestedTypes();

        foreach (var type in nestedTypes)
        {
            var groupName = type.Name;

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (var field in fields)
            {
                if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
                {
                    var code = field.GetValue(null)?.ToString();
                    var name = field.Name;

                    // 嘗試讀取 [Description] 作為中文名稱，如果沒有就用變數名稱
                    var descriptionAttribute = field.GetCustomAttribute<DescriptionAttribute>();
                    var description = descriptionAttribute?.Description ?? name;

                    if (!string.IsNullOrEmpty(code))
                    {
                        yield return new PermissionInfo(code, description, groupName);
                    }
                }
            }
        }
    }
}
