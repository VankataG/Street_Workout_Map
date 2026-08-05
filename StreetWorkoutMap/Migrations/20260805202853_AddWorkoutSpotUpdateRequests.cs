using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreetWorkoutMap.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkoutSpotUpdateRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkoutSpotsUpdateRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkoutSpotId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    District = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    HasPullUpBars = table.Column<bool>(type: "boolean", nullable: false),
                    HasParallelBars = table.Column<bool>(type: "boolean", nullable: false),
                    HasRings = table.Column<bool>(type: "boolean", nullable: false),
                    HasLighting = table.Column<bool>(type: "boolean", nullable: false),
                    IsIndoor = table.Column<bool>(type: "boolean", nullable: false),
                    SubmittedByUserId = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSpotsUpdateRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutSpotsUpdateRequests_AspNetUsers_SubmittedByUserId",
                        column: x => x.SubmittedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutSpotsUpdateRequests_WorkoutSpots_WorkoutSpotId",
                        column: x => x.WorkoutSpotId,
                        principalTable: "WorkoutSpots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutSpotUpdateImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoragePath = table.Column<string>(type: "text", nullable: false),
                    WorkoutSpotUpdateRequestId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSpotUpdateImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutSpotUpdateImages_WorkoutSpotsUpdateRequests_WorkoutS~",
                        column: x => x.WorkoutSpotUpdateRequestId,
                        principalTable: "WorkoutSpotsUpdateRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSpotsUpdateRequests_SubmittedByUserId",
                table: "WorkoutSpotsUpdateRequests",
                column: "SubmittedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSpotsUpdateRequests_WorkoutSpotId",
                table: "WorkoutSpotsUpdateRequests",
                column: "WorkoutSpotId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSpotUpdateImages_WorkoutSpotUpdateRequestId",
                table: "WorkoutSpotUpdateImages",
                column: "WorkoutSpotUpdateRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkoutSpotUpdateImages");

            migrationBuilder.DropTable(
                name: "WorkoutSpotsUpdateRequests");
        }
    }
}
