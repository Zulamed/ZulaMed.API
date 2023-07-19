using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZulaMed.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class VideoComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Like = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Dislike = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    SentById = table.Column<Guid>(type: "uuid", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RelatedVideoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_User_SentById",
                        column: x => x.SentById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comment_Video_RelatedVideoId",
                        column: x => x.RelatedVideoId,
                        principalTable: "Video",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_RelatedVideoId",
                table: "Comment",
                column: "RelatedVideoId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_SentById",
                table: "Comment",
                column: "SentById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");
        }
    }
}
