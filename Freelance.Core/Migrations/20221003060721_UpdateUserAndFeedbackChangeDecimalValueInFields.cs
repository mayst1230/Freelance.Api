using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Freelance.Core.Migrations
{
    public partial class UpdateUserAndFeedbackChangeDecimalValueInFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "rating",
                schema: "freelance",
                table: "users",
                type: "numeric(2,1)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(1,1)");

            migrationBuilder.AlterColumn<decimal>(
                name: "user_rating",
                schema: "freelance",
                table: "feedbacks",
                type: "numeric(2,1)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(1,1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "rating",
                schema: "freelance",
                table: "users",
                type: "numeric(1,1)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(2,1)");

            migrationBuilder.AlterColumn<decimal>(
                name: "user_rating",
                schema: "freelance",
                table: "feedbacks",
                type: "numeric(1,1)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(2,1)");
        }
    }
}
