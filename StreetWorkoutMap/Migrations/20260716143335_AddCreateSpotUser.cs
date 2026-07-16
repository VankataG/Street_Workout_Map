using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreetWorkoutMap.Migrations
{
    /// <inheritdoc />
    public partial class AddCreateSpotUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubmittedByUserId",
                table: "WorkoutSpots",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSpots_SubmittedByUserId",
                table: "WorkoutSpots",
                column: "SubmittedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutSpots_AspNetUsers_SubmittedByUserId",
                table: "WorkoutSpots",
                column: "SubmittedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutSpots_AspNetUsers_SubmittedByUserId",
                table: "WorkoutSpots");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutSpots_SubmittedByUserId",
                table: "WorkoutSpots");

            migrationBuilder.DropColumn(
                name: "SubmittedByUserId",
                table: "WorkoutSpots");
        }
    }
}
