using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khadamati.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class PlatformDefaultsUsdLebanon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "SubscriptionPlans",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3,
                oldDefaultValue: "SAR");

            migrationBuilder.AlterColumn<string>(
                name: "Timezone",
                table: "UserProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Asia/Beirut",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Asia/Riyadh");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Currency",
                table: "SubscriptionPlans",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "SAR",
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "Timezone",
                table: "UserProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Asia/Riyadh",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Asia/Beirut");
        }
    }
}
