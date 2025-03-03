using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bet_Oven.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveForeignKeyFromFavoriteLeagues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteLeagues_LeagueInfo_LeagueId",
                table: "FavoriteLeagues");

            migrationBuilder.DropTable(
                name: "LeagueInfo");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteLeagues_LeagueId",
                table: "FavoriteLeagues");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeagueInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueInfo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteLeagues_LeagueId",
                table: "FavoriteLeagues",
                column: "LeagueId");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteLeagues_LeagueInfo_LeagueId",
                table: "FavoriteLeagues",
                column: "LeagueId",
                principalTable: "LeagueInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
