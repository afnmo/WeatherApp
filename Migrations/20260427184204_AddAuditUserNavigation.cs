using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherApp.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditUserNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ChangedByUserId",
                table: "WeatherAudits",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_WeatherAudits_ChangedByUserId",
                table: "WeatherAudits",
                column: "ChangedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WeatherAudits_AspNetUsers_ChangedByUserId",
                table: "WeatherAudits",
                column: "ChangedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeatherAudits_AspNetUsers_ChangedByUserId",
                table: "WeatherAudits");

            migrationBuilder.DropIndex(
                name: "IX_WeatherAudits_ChangedByUserId",
                table: "WeatherAudits");

            migrationBuilder.AlterColumn<string>(
                name: "ChangedByUserId",
                table: "WeatherAudits",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
