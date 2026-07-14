using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreetWorkoutMap.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkoutSpots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSpots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpotImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoragePath = table.Column<string>(type: "text", nullable: false),
                    WorkoutSpotId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpotImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpotImages_WorkoutSpots_WorkoutSpotId",
                        column: x => x.WorkoutSpotId,
                        principalTable: "WorkoutSpots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpotImages_WorkoutSpotId",
                table: "SpotImages",
                column: "WorkoutSpotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpotImages");

            migrationBuilder.DropTable(
                name: "WorkoutSpots");
        }
    }
}
