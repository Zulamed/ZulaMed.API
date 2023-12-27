using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZulaMed.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRestrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_User_SubscriberId",
                table: "Subscription");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_User_SubscriberId",
                table: "Subscription",
                column: "SubscriberId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_User_SubscriberId",
                table: "Subscription");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_User_SubscriberId",
                table: "Subscription",
                column: "SubscriberId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
