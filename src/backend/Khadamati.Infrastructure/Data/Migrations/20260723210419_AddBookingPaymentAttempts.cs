using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khadamati.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingPaymentAttempts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrentAttemptId",
                table: "BookingPayments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FailedAt",
                table: "BookingPayments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GatewaySessionId",
                table: "BookingPayments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GatewayTransactionId",
                table: "BookingPayments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentProvider",
                table: "BookingPayments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Development");

            migrationBuilder.Sql("""
                UPDATE BookingPayments
                SET PaymentProvider = CASE
                    WHEN TransactionReference LIKE 'KHD-%' THEN 'Development'
                    WHEN TransactionReference IS NOT NULL AND LTRIM(RTRIM(TransactionReference)) <> '' THEN 'Moyasar'
                    ELSE 'Development'
                END;
                """);

            migrationBuilder.Sql("""
                ;WITH ranked AS (
                    SELECT Id,
                           ROW_NUMBER() OVER (
                               PARTITION BY TransactionReference
                               ORDER BY CreatedDate DESC, Id DESC) AS rn
                    FROM BookingPayments
                    WHERE TransactionReference IS NOT NULL
                )
                UPDATE p
                SET TransactionReference = NULL
                FROM BookingPayments p
                INNER JOIN ranked r ON r.Id = p.Id
                WHERE r.rn > 1;
                """);

            migrationBuilder.CreateTable(
                name: "BookingPaymentAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingPaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentProvider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false),
                    GatewaySessionId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GatewayTransactionId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    WebhookEventId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingPaymentAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingPaymentAttempts_BookingPayments_BookingPaymentId",
                        column: x => x.BookingPaymentId,
                        principalTable: "BookingPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql("""
                INSERT INTO BookingPaymentAttempts (
                    Id, BookingPaymentId, PaymentProvider, AttemptNumber,
                    GatewaySessionId, GatewayTransactionId, Status, Amount, Currency,
                    RequestDate, CompletedDate, FailedDate, FailureReason, WebhookEventId,
                    CreatedDate, ModifiedDate, Deleted, DeletedDate, CreatedBy, ModifiedBy, DeletedBy)
                SELECT
                    NEWID(),
                    p.Id,
                    p.PaymentProvider,
                    1,
                    CASE WHEN p.TransactionReference LIKE 'KHD-%' THEN p.TransactionReference ELSE NULL END,
                    CASE WHEN p.TransactionReference LIKE 'KHD-%' THEN NULL ELSE p.TransactionReference END,
                    p.Status,
                    p.Amount,
                    p.Currency,
                    p.CreatedDate,
                    p.PaidAt,
                    NULL,
                    CASE WHEN LEN(ISNULL(p.FailureReason, '')) > 1000 THEN LEFT(p.FailureReason, 1000) ELSE p.FailureReason END,
                    NULL,
                    p.CreatedDate,
                    p.ModifiedDate,
                    0,
                    NULL,
                    p.CreatedBy,
                    p.ModifiedBy,
                    NULL
                FROM BookingPayments p
                WHERE p.Deleted = 0
                  AND NOT EXISTS (
                      SELECT 1 FROM BookingPaymentAttempts a WHERE a.BookingPaymentId = p.Id);
                """);

            migrationBuilder.Sql("""
                UPDATE p
                SET
                    CurrentAttemptId = a.Id,
                    GatewaySessionId = a.GatewaySessionId,
                    GatewayTransactionId = a.GatewayTransactionId
                FROM BookingPayments p
                INNER JOIN BookingPaymentAttempts a ON a.BookingPaymentId = p.Id AND a.AttemptNumber = 1
                WHERE p.CurrentAttemptId IS NULL;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_BookingPayments_CurrentAttemptId",
                table: "BookingPayments",
                column: "CurrentAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPayments_GatewayTransactionId",
                table: "BookingPayments",
                column: "GatewayTransactionId",
                filter: "[GatewayTransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPayments_TransactionReference",
                table: "BookingPayments",
                column: "TransactionReference",
                unique: true,
                filter: "[TransactionReference] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPaymentAttempts_BookingPaymentId_AttemptNumber",
                table: "BookingPaymentAttempts",
                columns: new[] { "BookingPaymentId", "AttemptNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingPaymentAttempts_GatewayTransactionId",
                table: "BookingPaymentAttempts",
                column: "GatewayTransactionId",
                filter: "[GatewayTransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPaymentAttempts_WebhookEventId",
                table: "BookingPaymentAttempts",
                column: "WebhookEventId",
                unique: true,
                filter: "[WebhookEventId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_BookingPaymentAttempts_OneCompletedPerPayment",
                table: "BookingPaymentAttempts",
                column: "BookingPaymentId",
                unique: true,
                filter: "[Status] = 3 AND [Deleted] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingPayments_BookingPaymentAttempts_CurrentAttemptId",
                table: "BookingPayments",
                column: "CurrentAttemptId",
                principalTable: "BookingPaymentAttempts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingPayments_BookingPaymentAttempts_CurrentAttemptId",
                table: "BookingPayments");

            migrationBuilder.DropTable(
                name: "BookingPaymentAttempts");

            migrationBuilder.DropIndex(
                name: "IX_BookingPayments_CurrentAttemptId",
                table: "BookingPayments");

            migrationBuilder.DropIndex(
                name: "IX_BookingPayments_GatewayTransactionId",
                table: "BookingPayments");

            migrationBuilder.DropIndex(
                name: "IX_BookingPayments_TransactionReference",
                table: "BookingPayments");

            migrationBuilder.DropColumn(
                name: "CurrentAttemptId",
                table: "BookingPayments");

            migrationBuilder.DropColumn(
                name: "FailedAt",
                table: "BookingPayments");

            migrationBuilder.DropColumn(
                name: "GatewaySessionId",
                table: "BookingPayments");

            migrationBuilder.DropColumn(
                name: "GatewayTransactionId",
                table: "BookingPayments");

            migrationBuilder.DropColumn(
                name: "PaymentProvider",
                table: "BookingPayments");
        }
    }
}
