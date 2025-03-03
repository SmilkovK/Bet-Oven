using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bet_Oven.Data.Migrations
{
    /// <inheritdoc />
    public partial class FavoriteLeaguesaddition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeagueInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteLeagues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LeagueId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteLeagues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteLeagues_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteLeagues_LeagueInfo_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "LeagueInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteLeagues_LeagueId",
                table: "FavoriteLeagues",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteLeagues_UserId",
                table: "FavoriteLeagues",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteLeagues");

            migrationBuilder.DropTable(
                name: "LeagueInfo");
        }
    }
}
