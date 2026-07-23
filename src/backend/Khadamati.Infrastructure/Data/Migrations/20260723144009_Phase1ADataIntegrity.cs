using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khadamati.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Phase1ADataIntegrity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Coupons_Code",
                table: "Coupons");

            migrationBuilder.RenameIndex(
                name: "IX_BookingSlotReservations_CraftsmanId_SlotStart",
                table: "BookingSlotReservations",
                newName: "IX_BookingSlotReservations_ActiveSlot");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "ServiceRequests",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Coupons",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_CraftsmanId_Status_ScheduledAt",
                table: "ServiceRequests",
                columns: new[] { "CraftsmanId", "Status", "ScheduledAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Code_Active",
                table: "Coupons",
                column: "Code",
                unique: true,
                filter: "[Deleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSlotReservations_CraftsmanId_IsActive_SlotRange",
                table: "BookingSlotReservations",
                columns: new[] { "CraftsmanId", "IsActive", "SlotStart", "SlotEnd" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_CraftsmanId_Status_ScheduledAt",
                table: "ServiceRequests");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_Code_Active",
                table: "Coupons");

            migrationBuilder.DropIndex(
                name: "IX_BookingSlotReservations_CraftsmanId_IsActive_SlotRange",
                table: "BookingSlotReservations");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Coupons");

            migrationBuilder.RenameIndex(
                name: "IX_BookingSlotReservations_ActiveSlot",
                table: "BookingSlotReservations",
                newName: "IX_BookingSlotReservations_CraftsmanId_SlotStart");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Code",
                table: "Coupons",
                column: "Code",
                unique: true);
        }
    }
}
