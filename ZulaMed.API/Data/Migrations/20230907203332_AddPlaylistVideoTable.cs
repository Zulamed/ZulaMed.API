using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZulaMed.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaylistVideoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Video_Playlist_PlaylistId",
                table: "Video");

            migrationBuilder.DropIndex(
                name: "IX_Video_PlaylistId",
                table: "Video");

            migrationBuilder.DropColumn(
                name: "PlaylistId",
                table: "Video");

            migrationBuilder.CreateTable(
                name: "PlaylistVideo",
                columns: table => new
                {
                    PlaylistsId = table.Column<Guid>(type: "uuid", nullable: false),
                    VideosId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistVideo", x => new { x.PlaylistsId, x.VideosId });
                    table.ForeignKey(
                        name: "FK_PlaylistVideo_Playlist_PlaylistsId",
                        column: x => x.PlaylistsId,
                        principalTable: "Playlist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistVideo_Video_VideosId",
                        column: x => x.VideosId,
                        principalTable: "Video",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistVideo_VideosId",
                table: "PlaylistVideo",
                column: "VideosId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaylistVideo");

            migrationBuilder.AddColumn<Guid>(
                name: "PlaylistId",
                table: "Video",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Video_PlaylistId",
                table: "Video",
                column: "PlaylistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Video_Playlist_PlaylistId",
                table: "Video",
                column: "PlaylistId",
                principalTable: "Playlist",
                principalColumn: "Id");
        }
    }
}
