using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Freelance.Core.Migrations
{
    public partial class UpdateOrderAddVirtualUsersFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_orders_users_user_id",
                schema: "freelance",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "fk_orders_users_user_id1",
                schema: "freelance",
                table: "orders");

            migrationBuilder.AddForeignKey(
                name: "fk_orders_users_contractor_id",
                schema: "freelance",
                table: "orders",
                column: "contractor_id",
                principalSchema: "freelance",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_orders_users_customer_id",
                schema: "freelance",
                table: "orders",
                column: "customer_id",
                principalSchema: "freelance",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_orders_users_contractor_id",
                schema: "freelance",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "fk_orders_users_customer_id",
                schema: "freelance",
                table: "orders");

            migrationBuilder.AddForeignKey(
                name: "fk_orders_users_user_id",
                schema: "freelance",
                table: "orders",
                column: "contractor_id",
                principalSchema: "freelance",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_orders_users_user_id1",
                schema: "freelance",
                table: "orders",
                column: "customer_id",
                principalSchema: "freelance",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
