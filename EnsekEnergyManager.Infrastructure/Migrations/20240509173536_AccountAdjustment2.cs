using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnsekEnergyManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AccountAdjustment2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Accounts",
                newName: "FirstName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Accounts",
                newName: "Name");
        }
    }
}
