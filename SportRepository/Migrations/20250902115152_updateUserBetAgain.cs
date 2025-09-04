using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportRepository.Migrations
{
    /// <inheritdoc />
    public partial class updateUserBetAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "UserBets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "UserBets");
        }
    }
}
