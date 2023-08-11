using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZulaMed.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToLikeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LikeVideo",
                table: "LikeVideo");

            migrationBuilder.AddColumn<Guid>(
                name: "LikedById",
                table: "LikeVideo",
                type: "uuid",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LikeVideo",
                table: "LikeVideo",
                columns: new[] { "Id", "ParentId", "LikedById" });

            migrationBuilder.CreateIndex(
                name: "IX_LikeVideo_LikedById",
                table: "LikeVideo",
                column: "LikedById");

            migrationBuilder.AddForeignKey(
                name: "FK_LikeVideo_User_LikedById",
                table: "LikeVideo",
                column: "LikedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LikeVideo_User_LikedById",
                table: "LikeVideo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LikeVideo",
                table: "LikeVideo");

            migrationBuilder.DropIndex(
                name: "IX_LikeVideo_LikedById",
                table: "LikeVideo");

            migrationBuilder.DropColumn(
                name: "LikedById",
                table: "LikeVideo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LikeVideo",
                table: "LikeVideo",
                columns: new[] { "Id", "ParentId", "LikedAt" });
        }
    }
}
