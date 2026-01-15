using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IK_Intranet_App.Migrations
{
    /// <inheritdoc />
    public partial class AdSoyadSistemi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdSoyad",
                table: "AspNetUsers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdSoyad",
                table: "AspNetUsers");
        }
    }
}
