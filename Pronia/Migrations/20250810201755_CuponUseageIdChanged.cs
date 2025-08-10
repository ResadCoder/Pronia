using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pronia.Migrations
{
    /// <inheritdoc />
    public partial class CuponUseageIdChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CuponUsages_AspNetUsers_UserId1",
                table: "CuponUsages");

            migrationBuilder.DropIndex(
                name: "IX_CuponUsages_UserId1",
                table: "CuponUsages");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "CuponUsages");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "CuponUsages",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_CuponUsages_UserId",
                table: "CuponUsages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CuponUsages_AspNetUsers_UserId",
                table: "CuponUsages",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CuponUsages_AspNetUsers_UserId",
                table: "CuponUsages");

            migrationBuilder.DropIndex(
                name: "IX_CuponUsages_UserId",
                table: "CuponUsages");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CuponUsages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "CuponUsages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CuponUsages_UserId1",
                table: "CuponUsages",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CuponUsages_AspNetUsers_UserId1",
                table: "CuponUsages",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
