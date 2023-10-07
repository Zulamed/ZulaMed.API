using Microsoft.EntityFrameworkCore.Migrations;
using ZulaMed.API.Domain.Video;

#nullable disable

namespace ZulaMed.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoStatusAndMakeSomePropertiesNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:video_status", "waiting_for_upload,getting_processed,ready,cancelled");

            migrationBuilder.AlterColumn<string>(
                name: "VideoUrl",
                table: "Video",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "VideoTitle",
                table: "Video",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "VideoThumbnail",
                table: "Video",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "VideoDescription",
                table: "Video",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<VideoStatus>(
                name: "VideoStatus",
                table: "Video",
                type: "video_status",
                nullable: false,
                defaultValue: VideoStatus.WaitingForUpload);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoStatus",
                table: "Video");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:video_status", "waiting_for_upload,getting_processed,ready,cancelled");

            migrationBuilder.AlterColumn<string>(
                name: "VideoUrl",
                table: "Video",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VideoTitle",
                table: "Video",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VideoThumbnail",
                table: "Video",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VideoDescription",
                table: "Video",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
