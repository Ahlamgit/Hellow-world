using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khadamati.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AreebaPaymentJsFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FailedAt",
                table: "BookingPaymentAttempts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GatewayStatus",
                table: "BookingPaymentAttempts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MerchantTransactionId",
                table: "BookingPaymentAttempts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProviderUuid",
                table: "BookingPaymentAttempts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RedirectUrl",
                table: "BookingPaymentAttempts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "BookingPaymentAttempts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RetryReason",
                table: "BookingPaymentAttempts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnType",
                table: "BookingPaymentAttempts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThreeDSReference",
                table: "BookingPaymentAttempts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionToken",
                table: "BookingPaymentAttempts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebhookEventId",
                table: "BookingPaymentAttempts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingPaymentAttempts_MerchantTransactionId",
                table: "BookingPaymentAttempts",
                column: "MerchantTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPaymentAttempts_ProviderUuid",
                table: "BookingPaymentAttempts",
                column: "ProviderUuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookingPaymentAttempts_MerchantTransactionId",
                table: "BookingPaymentAttempts");

            migrationBuilder.DropIndex(
                name: "IX_BookingPaymentAttempts_ProviderUuid",
                table: "BookingPaymentAttempts");

            migrationBuilder.DropColumn(
                name: "FailedAt",
                table: "BookingPaymentAttempts");

            migrationBuilder.DropColumn(
                name: "GatewayStatus",
                table: "BookingPaymentAttempts");

            migrationBuilder.DropColumn(
                name: "MerchantTransactionId",
                table: "BookingPaymentAttempts");

            migrationBuilder.DropColumn(
                name: "ProviderUuid",
                table: "BookingPaymentAttempts");

            migrationBuilder.DropColumn(
                name: "RedirectUrl",
                table: "BookingPaymentAttempts");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "BookingPaymentAttempts");

            migrationBuilder.DropColumn(
                name: "RetryReason",
                table: "BookingPaymentAttempts");

            migrationBuilder.DropColumn(
                name: "ReturnType",
                table: "BookingPaymentAttempts");

            migrationBuilder.DropColumn(
                name: "ThreeDSReference",
                table: "BookingPaymentAttempts");

            migrationBuilder.DropColumn(
                name: "TransactionToken",
                table: "BookingPaymentAttempts");

            migrationBuilder.DropColumn(
                name: "WebhookEventId",
                table: "BookingPaymentAttempts");
        }
    }
}
