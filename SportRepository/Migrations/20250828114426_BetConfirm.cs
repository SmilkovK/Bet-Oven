using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportRepository.Migrations
{
    /// <inheritdoc />
    public partial class BetConfirm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BetConfirmId",
                table: "UserBets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BetConfirms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlacedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BetConfirms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BetConfirms_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBets_BetConfirmId",
                table: "UserBets",
                column: "BetConfirmId");

            migrationBuilder.CreateIndex(
                name: "IX_BetConfirms_UserId",
                table: "BetConfirms",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBets_BetConfirms_BetConfirmId",
                table: "UserBets",
                column: "BetConfirmId",
                principalTable: "BetConfirms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBets_BetConfirms_BetConfirmId",
                table: "UserBets");

            migrationBuilder.DropTable(
                name: "BetConfirms");

            migrationBuilder.DropIndex(
                name: "IX_UserBets_BetConfirmId",
                table: "UserBets");

            migrationBuilder.DropColumn(
                name: "BetConfirmId",
                table: "UserBets");
        }
    }
}
