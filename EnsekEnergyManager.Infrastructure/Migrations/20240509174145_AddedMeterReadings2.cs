using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnsekEnergyManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedMeterReadings2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MeterReadings_AccountId",
                table: "MeterReadings",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_MeterReadings_Accounts_AccountId",
                table: "MeterReadings",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeterReadings_Accounts_AccountId",
                table: "MeterReadings");

            migrationBuilder.DropIndex(
                name: "IX_MeterReadings_AccountId",
                table: "MeterReadings");
        }
    }
}
