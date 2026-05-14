using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChronosPoint.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersAndAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CHRONOSPOINT_USERS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FullName = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "NVARCHAR2(512)", maxLength: 512, nullable: false),
                    IsActive = table.Column<int>(type: "NUMBER(1)", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHRONOSPOINT_USERS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CHRONOSPOINT_USER_TENANTS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UserId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TenantId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Role = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHRONOSPOINT_USER_TENANTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CHRONOSPOINT_USER_TENANTS_CHRONOSPOINT_TENANTS_TenantId",
                        column: x => x.TenantId,
                        principalTable: "CHRONOSPOINT_TENANTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CHRONOSPOINT_USER_TENANTS_CHRONOSPOINT_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "CHRONOSPOINT_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CHRONOSPOINT_USER_TENANTS_TenantId",
                table: "CHRONOSPOINT_USER_TENANTS",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CHRONOSPOINT_USER_TENANTS_UserId_TenantId",
                table: "CHRONOSPOINT_USER_TENANTS",
                columns: new[] { "UserId", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CHRONOSPOINT_USERS_Email",
                table: "CHRONOSPOINT_USERS",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CHRONOSPOINT_USER_TENANTS");

            migrationBuilder.DropTable(
                name: "CHRONOSPOINT_USERS");
        }
    }
}
