using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnnOtter.WayToSecureExchange.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSaltToNonce : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Salt",
                table: "Secrets",
                newName: "Nonce");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nonce",
                table: "Secrets",
                newName: "Salt");
        }
    }
}
