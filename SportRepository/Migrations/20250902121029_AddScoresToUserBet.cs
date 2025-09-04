using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportRepository.Migrations
{
    /// <inheritdoc />
    public partial class AddScoresToUserBet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AwayGoals",
                table: "UserBets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomeGoals",
                table: "UserBets",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwayGoals",
                table: "UserBets");

            migrationBuilder.DropColumn(
                name: "HomeGoals",
                table: "UserBets");
        }
    }
}
