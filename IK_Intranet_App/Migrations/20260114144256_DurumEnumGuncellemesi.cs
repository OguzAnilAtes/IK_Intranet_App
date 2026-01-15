using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IK_Intranet_App.Migrations
{
    /// <inheritdoc />
    public partial class DurumEnumGuncellemesi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Önce eski "String" (Yazı) olan sütunu komple siliyoruz.
            // (Böylece içindeki uyumsuz veriler gidiyor, hata çıkmıyor)
            migrationBuilder.DropColumn(
                name: "Durum",
                table: "Gorevler");

            // 2. Şimdi "Int" (Sayı/Enum) olarak tertemiz yeniden ekliyoruz.
            migrationBuilder.AddColumn<int>(
                name: "Durum",
                table: "Gorevler",
                type: "integer",
                nullable: false,
                defaultValue: 0); // Varsayılan olarak 0 (Yapılacak) olsun
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Durum",
                table: "Gorevler",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);
        }
    }
}
