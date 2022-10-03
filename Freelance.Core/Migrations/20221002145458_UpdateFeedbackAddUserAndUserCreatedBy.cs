using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Freelance.Core.Migrations
{
    public partial class UpdateFeedbackAddUserAndUserCreatedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_feedbacks_users_user_id1",
                schema: "freelance",
                table: "feedbacks");

            migrationBuilder.AddForeignKey(
                name: "fk_feedbacks_users_created_by_user_id",
                schema: "freelance",
                table: "feedbacks",
                column: "created_by",
                principalSchema: "freelance",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_feedbacks_users_created_by_user_id",
                schema: "freelance",
                table: "feedbacks");

            migrationBuilder.AddForeignKey(
                name: "fk_feedbacks_users_user_id1",
                schema: "freelance",
                table: "feedbacks",
                column: "created_by",
                principalSchema: "freelance",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
