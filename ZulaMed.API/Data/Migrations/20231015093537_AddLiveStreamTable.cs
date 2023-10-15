using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Mux.Csharp.Sdk.Model;

#nullable disable

namespace ZulaMed.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLiveStreamTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:live_stream_status", "active,idle,disabled")
                .Annotation("Npgsql:Enum:video_status", "waiting_for_upload,getting_processed,ready,cancelled")
                .OldAnnotation("Npgsql:Enum:video_status", "waiting_for_upload,getting_processed,ready,cancelled");

            migrationBuilder.CreateTable(
                name: "LiveStream",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RelatedVideoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<LiveStreamStatus>(type: "live_stream_status", nullable: false, defaultValue: LiveStreamStatus.Idle),
                    PlaybackId = table.Column<string>(type: "text", nullable: false),
                    StreamKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveStream", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiveStream_Video_RelatedVideoId",
                        column: x => x.RelatedVideoId,
                        principalTable: "Video",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LiveStream_RelatedVideoId",
                table: "LiveStream",
                column: "RelatedVideoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LiveStream");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:video_status", "waiting_for_upload,getting_processed,ready,cancelled")
                .OldAnnotation("Npgsql:Enum:live_stream_status", "active,idle,disabled")
                .OldAnnotation("Npgsql:Enum:video_status", "waiting_for_upload,getting_processed,ready,cancelled");
        }
    }
}
