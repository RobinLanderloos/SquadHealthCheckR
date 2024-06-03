using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SquadHealthCheckR.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class VoteCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Votes",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_SquadMemberId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Votes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Votes",
                table: "Votes",
                columns: new[] { "SquadMemberId", "HealthIndicatorId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Votes",
                table: "Votes");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Votes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Votes",
                table: "Votes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_SquadMemberId",
                table: "Votes",
                column: "SquadMemberId");
        }
    }
}
