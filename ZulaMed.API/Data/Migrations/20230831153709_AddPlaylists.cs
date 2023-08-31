using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZulaMed.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaylists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PlaylistId",
                table: "Video",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Playlist",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlaylistName = table.Column<string>(type: "text", nullable: false),
                    PlaylistDescription = table.Column<string>(type: "text", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Playlist_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Video_PlaylistId",
                table: "Video",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_Playlist_OwnerId",
                table: "Playlist",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Video_Playlist_PlaylistId",
                table: "Video",
                column: "PlaylistId",
                principalTable: "Playlist",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Video_Playlist_PlaylistId",
                table: "Video");

            migrationBuilder.DropTable(
                name: "Playlist");

            migrationBuilder.DropIndex(
                name: "IX_Video_PlaylistId",
                table: "Video");

            migrationBuilder.DropColumn(
                name: "PlaylistId",
                table: "Video");
        }
    }
}
