using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Freelance.Core.Migrations
{
    public partial class UpdateFilesAddCreatedByUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_files_file_id",
                schema: "freelance",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user",
                schema: "freelance",
                table: "user");

            migrationBuilder.RenameTable(
                name: "user",
                schema: "freelance",
                newName: "users",
                newSchema: "freelance");

            migrationBuilder.RenameIndex(
                name: "ix_user_photo_file_id",
                schema: "freelance",
                table: "users",
                newName: "ix_users_photo_file_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                schema: "freelance",
                table: "users",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_files_created_by",
                schema: "freelance",
                table: "files",
                column: "created_by");

            migrationBuilder.AddForeignKey(
                name: "fk_files_users_user_id",
                schema: "freelance",
                table: "files",
                column: "created_by",
                principalSchema: "freelance",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_files_users_user_id",
                schema: "freelance",
                table: "files");

            migrationBuilder.DropForeignKey(
                name: "fk_users_files_file_id",
                schema: "freelance",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_files_created_by",
                schema: "freelance",
                table: "files");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                schema: "freelance",
                table: "users");

            migrationBuilder.RenameTable(
                name: "users",
                schema: "freelance",
                newName: "user",
                newSchema: "freelance");

            migrationBuilder.RenameIndex(
                name: "ix_users_photo_file_id",
                schema: "freelance",
                table: "user",
                newName: "ix_user_photo_file_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user",
                schema: "freelance",
                table: "user",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_files_file_id",
                schema: "freelance",
                table: "user",
                column: "photo_file_id",
                principalSchema: "freelance",
                principalTable: "files",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
