using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IK_Intranet_App.Migrations
{
    /// <inheritdoc />
    public partial class IliskiselYapiyaGecis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AtananKisi",
                table: "Gorevler",
                newName: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Gorevler_AppUserId",
                table: "Gorevler",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gorevler_AspNetUsers_AppUserId",
                table: "Gorevler",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gorevler_AspNetUsers_AppUserId",
                table: "Gorevler");

            migrationBuilder.DropIndex(
                name: "IX_Gorevler_AppUserId",
                table: "Gorevler");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "Gorevler",
                newName: "AtananKisi");
        }
    }
}
