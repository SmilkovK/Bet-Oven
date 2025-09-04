using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportRepository.Migrations
{
    /// <inheritdoc />
    public partial class UserBet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FixtureId",
                table: "UserBets",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FixtureId",
                table: "UserBets");
        }
    }
}
