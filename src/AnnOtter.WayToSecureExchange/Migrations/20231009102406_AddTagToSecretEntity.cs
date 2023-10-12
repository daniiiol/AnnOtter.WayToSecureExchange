using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnnOtter.WayToSecureExchange.Migrations
{
    /// <inheritdoc />
    public partial class AddTagToSecretEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "Secrets",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tag",
                table: "Secrets");
        }
    }
}
