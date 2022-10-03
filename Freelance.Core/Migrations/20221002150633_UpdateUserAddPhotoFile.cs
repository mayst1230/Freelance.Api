using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Freelance.Core.Migrations
{
    public partial class UpdateUserAddPhotoFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_files_file_id",
                schema: "freelance",
                table: "users");

            migrationBuilder.AddForeignKey(
                name: "fk_users_files_photo_file_id",
                schema: "freelance",
                table: "users",
                column: "photo_file_id",
                principalSchema: "freelance",
                principalTable: "files",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_files_photo_file_id",
                schema: "freelance",
                table: "users");

            migrationBuilder.AddForeignKey(
                name: "fk_users_files_file_id",
                schema: "freelance",
                table: "users",
                column: "photo_file_id",
                principalSchema: "freelance",
                principalTable: "files",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
