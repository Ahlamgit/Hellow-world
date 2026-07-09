using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khadamati.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixRescheduledFromCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_ServiceRequests_RescheduledFromId",
                table: "ServiceRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_ServiceRequests_RescheduledFromId",
                table: "ServiceRequests",
                column: "RescheduledFromId",
                principalTable: "ServiceRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_ServiceRequests_RescheduledFromId",
                table: "ServiceRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_ServiceRequests_RescheduledFromId",
                table: "ServiceRequests",
                column: "RescheduledFromId",
                principalTable: "ServiceRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
