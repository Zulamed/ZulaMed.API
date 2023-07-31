using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZulaMed.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixPublisher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VideoPublisherId",
                table: "Video",
                newName: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_Video_PublisherId",
                table: "Video",
                column: "PublisherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Video_User_PublisherId",
                table: "Video",
                column: "PublisherId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Video_User_PublisherId",
                table: "Video");

            migrationBuilder.DropIndex(
                name: "IX_Video_PublisherId",
                table: "Video");

            migrationBuilder.RenameColumn(
                name: "PublisherId",
                table: "Video",
                newName: "VideoPublisherId");
        }
    }
}
