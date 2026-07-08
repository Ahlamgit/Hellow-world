using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khadamati.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AuthModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Users",
                newName: "ModifiedBy");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Users",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Users",
                newName: "Deleted");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "Users",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Users",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "UserProfiles",
                newName: "ModifiedBy");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "UserProfiles",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "UserProfiles",
                newName: "Deleted");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "UserProfiles",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "UserProfiles",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "StoreProfiles",
                newName: "ModifiedBy");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "StoreProfiles",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "StoreProfiles",
                newName: "Deleted");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "StoreProfiles",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "StoreProfiles",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "StoreProducts",
                newName: "ModifiedBy");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "StoreProducts",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "StoreProducts",
                newName: "Deleted");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "StoreProducts",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "StoreProducts",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Services",
                newName: "ModifiedBy");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Services",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Services",
                newName: "Deleted");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "Services",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Services",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "ServiceRequests",
                newName: "ModifiedBy");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "ServiceRequests",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "ServiceRequests",
                newName: "Deleted");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "ServiceRequests",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ServiceRequests",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "ServiceCategories",
                newName: "ModifiedBy");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "ServiceCategories",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "ServiceCategories",
                newName: "Deleted");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "ServiceCategories",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ServiceCategories",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "RefreshTokens",
                newName: "ModifiedBy");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "RefreshTokens",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "RefreshTokens",
                newName: "Deleted");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "RefreshTokens",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "RefreshTokens",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "CraftsmanServices",
                newName: "ModifiedBy");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "CraftsmanServices",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "CraftsmanServices",
                newName: "Deleted");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "CraftsmanServices",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "CraftsmanServices",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "CraftsmanProfiles",
                newName: "ModifiedBy");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "CraftsmanProfiles",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "CraftsmanProfiles",
                newName: "Deleted");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "CraftsmanProfiles",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "CraftsmanProfiles",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Addresses",
                newName: "ModifiedBy");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Addresses",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Addresses",
                newName: "Deleted");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "Addresses",
                newName: "DeletedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Addresses",
                newName: "CreatedDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerifiedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PhoneVerifiedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RememberMe",
                table: "RefreshTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "ServiceRadiusKm",
                table: "CraftsmanProfiles",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "EmailVerificationTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_EmailVerificationTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailVerificationTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestedFromIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhoneOtpTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OtpHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    RequestedFromIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_PhoneOtpTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhoneOtpTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerificationTokens_ExpiresAt",
                table: "EmailVerificationTokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerificationTokens_UserId",
                table: "EmailVerificationTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_ExpiresAt",
                table: "PasswordResetTokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "PasswordResetTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneOtpTokens_ExpiresAt",
                table: "PhoneOtpTokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneOtpTokens_UserId_Phone",
                table: "PhoneOtpTokens",
                columns: new[] { "UserId", "Phone" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailVerificationTokens");

            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.DropTable(
                name: "PhoneOtpTokens");

            migrationBuilder.DropColumn(
                name: "EmailVerifiedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneVerifiedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RememberMe",
                table: "RefreshTokens");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "Users",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "Users",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "Users",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "Deleted",
                table: "Users",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "UserProfiles",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "UserProfiles",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "UserProfiles",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "Deleted",
                table: "UserProfiles",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "UserProfiles",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "StoreProfiles",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "StoreProfiles",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "StoreProfiles",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "Deleted",
                table: "StoreProfiles",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "StoreProfiles",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "StoreProducts",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "StoreProducts",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "StoreProducts",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "Deleted",
                table: "StoreProducts",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "StoreProducts",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "Services",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "Services",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "Services",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "Deleted",
                table: "Services",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Services",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "ServiceRequests",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "ServiceRequests",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "ServiceRequests",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "Deleted",
                table: "ServiceRequests",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "ServiceRequests",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "ServiceCategories",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "ServiceCategories",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "ServiceCategories",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "Deleted",
                table: "ServiceCategories",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "ServiceCategories",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "RefreshTokens",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "RefreshTokens",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "RefreshTokens",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "Deleted",
                table: "RefreshTokens",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "RefreshTokens",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "CraftsmanServices",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "CraftsmanServices",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "CraftsmanServices",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "Deleted",
                table: "CraftsmanServices",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "CraftsmanServices",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "CraftsmanProfiles",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "CraftsmanProfiles",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "CraftsmanProfiles",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "Deleted",
                table: "CraftsmanProfiles",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "CraftsmanProfiles",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "Addresses",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "Addresses",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "DeletedDate",
                table: "Addresses",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "Deleted",
                table: "Addresses",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Addresses",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<decimal>(
                name: "ServiceRadiusKm",
                table: "CraftsmanProfiles",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);
        }
    }
}
