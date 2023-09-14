using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZulaMed.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddViewHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ViewHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ViewedVideoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ViewedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViewHistory_User_ViewedById",
                        column: x => x.ViewedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ViewHistory_Video_ViewedVideoId",
                        column: x => x.ViewedVideoId,
                        principalTable: "Video",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ViewHistory_ViewedById",
                table: "ViewHistory",
                column: "ViewedById");

            migrationBuilder.CreateIndex(
                name: "IX_ViewHistory_ViewedVideoId",
                table: "ViewHistory",
                column: "ViewedVideoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ViewHistory");
        }
    }
}
