using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Freelance.Core.Migrations
{
    public partial class UpdateUserAddRestorePasswordCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "restore_password_code",
                schema: "freelance",
                table: "users",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "restore_password_code",
                schema: "freelance",
                table: "users");
        }
    }
}
