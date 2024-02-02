using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace firstApi.Migrations
{
    /// <inheritdoc />
    public partial class TenantTableUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenancyName",
                table: "Tenants",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenancyName",
                table: "Tenants");
        }
    }
}
