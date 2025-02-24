using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bet_Oven.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserAgreement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasAcceptedUserAgreement",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasAcceptedUserAgreement",
                table: "AspNetUsers");
        }
    }
}
