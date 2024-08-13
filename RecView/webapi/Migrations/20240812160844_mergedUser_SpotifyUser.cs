using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class mergedUser_SpotifyUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_SpotifyUsers_SpotifyUserId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "SpotifyUsers");

            migrationBuilder.DropIndex(
                name: "IX_Users_SpotifyUserId",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "SpotifyUserId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SpotifyUserId",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "SpotifyUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpotifyId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpotifyUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_SpotifyUserId",
                table: "Users",
                column: "SpotifyUserId",
                unique: true,
                filter: "[SpotifyUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_SpotifyUsers_SpotifyUserId",
                table: "Users",
                column: "SpotifyUserId",
                principalTable: "SpotifyUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
