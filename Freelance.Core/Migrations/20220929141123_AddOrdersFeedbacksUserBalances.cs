using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Freelance.Core.Migrations
{
    public partial class AddOrdersFeedbacksUserBalances : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "feedbacks",
                schema: "freelance",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    unique_identifier = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_feedbacks", x => x.id);
                    table.ForeignKey(
                        name: "fk_feedbacks_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "freelance",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_feedbacks_users_user_id1",
                        column: x => x.created_by,
                        principalSchema: "freelance",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "freelance",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    unique_identifier = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    contractor_id = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    customer_file_id = table.Column<int>(type: "integer", nullable: true),
                    contractor_file_id = table.Column<int>(type: "integer", nullable: true),
                    price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    started = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    canceled = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_orders", x => x.id);
                    table.ForeignKey(
                        name: "fk_orders_files_file_id",
                        column: x => x.contractor_file_id,
                        principalSchema: "freelance",
                        principalTable: "files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_orders_files_file_id1",
                        column: x => x.customer_file_id,
                        principalSchema: "freelance",
                        principalTable: "files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_orders_users_user_id",
                        column: x => x.contractor_id,
                        principalSchema: "freelance",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_orders_users_user_id1",
                        column: x => x.customer_id,
                        principalSchema: "freelance",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_balances",
                schema: "freelance",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    balance = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_balances", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_user_balances_users_user_id1",
                        column: x => x.user_id,
                        principalSchema: "freelance",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_balance_logs",
                schema: "freelance",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    unique_identifier = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    balance_before = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    balance_after = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    debit = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    credit = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_balance_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_balance_logs_user_balances_user_balance_user_id",
                        column: x => x.user_id,
                        principalSchema: "freelance",
                        principalTable: "user_balances",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_feedbacks_created_by",
                schema: "freelance",
                table: "feedbacks",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_feedbacks_user_id",
                schema: "freelance",
                table: "feedbacks",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_uq_feedback",
                schema: "freelance",
                table: "feedbacks",
                column: "unique_identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_orders_contractor_file_id",
                schema: "freelance",
                table: "orders",
                column: "contractor_file_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_contractor_id",
                schema: "freelance",
                table: "orders",
                column: "contractor_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_customer_file_id",
                schema: "freelance",
                table: "orders",
                column: "customer_file_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_customer_id",
                schema: "freelance",
                table: "orders",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_uq_order",
                schema: "freelance",
                table: "orders",
                column: "unique_identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_uq_user_balance_log",
                schema: "freelance",
                table: "user_balance_logs",
                column: "unique_identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_balance_logs_user_id",
                schema: "freelance",
                table: "user_balance_logs",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "feedbacks",
                schema: "freelance");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "freelance");

            migrationBuilder.DropTable(
                name: "user_balance_logs",
                schema: "freelance");

            migrationBuilder.DropTable(
                name: "user_balances",
                schema: "freelance");
        }
    }
}
