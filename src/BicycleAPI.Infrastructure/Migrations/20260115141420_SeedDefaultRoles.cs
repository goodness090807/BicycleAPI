using System;
using BicycleAPI.Domain.Variables;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BicycleAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var now = DateTime.UtcNow;

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name", "Code", "Description", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy" },
                values: new object[] { Roles.MemberId, Roles.MemberName, Roles.MemberCode, "一般會員角色", now, "System", null, null }
            );

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name", "Code", "Description", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy" },
                values: new object[] { Roles.TechnicianId, Roles.TechnicianName, Roles.TechnicianCode, "技師角色", now, "System", null, null }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: Roles.MemberId
            );

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: Roles.TechnicianId
            );
        }
    }
}
