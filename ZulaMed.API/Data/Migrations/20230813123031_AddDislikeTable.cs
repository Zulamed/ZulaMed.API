using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZulaMed.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDislikeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dislike<Video>",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: false),
                    DislikedById = table.Column<Guid>(type: "uuid", nullable: false),
                    DislikedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dislike<Video>", x => new { x.Id, x.ParentId, x.DislikedById });
                    table.ForeignKey(
                        name: "FK_Dislike<Video>_User_DislikedById",
                        column: x => x.DislikedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dislike<Video>_Video_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Video",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dislike<Video>_DislikedById",
                table: "Dislike<Video>",
                column: "DislikedById");

            migrationBuilder.CreateIndex(
                name: "IX_Dislike<Video>_ParentId",
                table: "Dislike<Video>",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dislike<Video>");
        }
    }
}
