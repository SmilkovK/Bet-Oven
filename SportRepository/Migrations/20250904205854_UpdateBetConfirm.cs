using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportRepository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBetConfirm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "CombinedPotentialWin",
                table: "BetConfirms",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CombinedPotentialWin",
                table: "BetConfirms");
        }
    }
}
