using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ZulaMed.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_SpecialtyGroup_GroupId",
                table: "User");

            migrationBuilder.DropTable(
                name: "SpecialtyGroup");

            migrationBuilder.DropIndex(
                name: "IX_User_GroupId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "University",
                table: "User");

            migrationBuilder.DropColumn(
                name: "WorkPlace",
                table: "User");

            migrationBuilder.CreateTable(
                name: "HospitalAccount",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountHospital = table.Column<string>(type: "text", nullable: false),
                    AccountAddress = table.Column<string>(type: "text", nullable: false),
                    AccountPostCode = table.Column<string>(type: "text", nullable: false),
                    AccountPhone = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HospitalAccount", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_HospitalAccount_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonalAccount",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountGender = table.Column<string>(type: "text", nullable: false),
                    AccountTitle = table.Column<string>(type: "text", nullable: false),
                    AccountCareerStage = table.Column<string>(type: "text", nullable: false),
                    AccountProfessionalActivity = table.Column<string>(type: "text", nullable: false),
                    AccountSpecialty = table.Column<string>(type: "text", nullable: false),
                    AccountDepartment = table.Column<string>(type: "text", nullable: false),
                    AccountBirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    AccountInstitute = table.Column<string>(type: "text", nullable: false),
                    AccountRole = table.Column<string>(type: "text", nullable: false),
                    PlacesOfWork = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalAccount", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_PersonalAccount_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UniversityAccount",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountUniversity = table.Column<string>(type: "text", nullable: false),
                    AccountAddress = table.Column<string>(type: "text", nullable: false),
                    AccountPostCode = table.Column<string>(type: "text", nullable: false),
                    AccountPhone = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniversityAccount", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UniversityAccount_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HospitalAccount");

            migrationBuilder.DropTable(
                name: "PersonalAccount");

            migrationBuilder.DropTable(
                name: "UniversityAccount");

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "User",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "University",
                table: "User",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WorkPlace",
                table: "User",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SpecialtyGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialtyGroup", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_GroupId",
                table: "User",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_SpecialtyGroup_GroupId",
                table: "User",
                column: "GroupId",
                principalTable: "SpecialtyGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
