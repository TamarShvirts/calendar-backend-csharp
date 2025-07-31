using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace calendarProject.Migrations
{
    public partial class AddusersAndcommentTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "docStatus",
                table: "document");

            migrationBuilder.AlterColumn<string>(
                name: "docName",
                table: "document",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "docConfirmed",
                table: "document",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "docEdit",
                table: "document",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "comment",
                columns: table => new
                {
                    commentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    commentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dateByCalendar = table.Column<DateTime>(type: "datetime2", nullable: false),
                    userName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comment", x => x.commentId);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    userNameId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.userNameId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comment");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropColumn(
                name: "docConfirmed",
                table: "document");

            migrationBuilder.DropColumn(
                name: "docEdit",
                table: "document");

            migrationBuilder.AlterColumn<string>(
                name: "docName",
                table: "document",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "docStatus",
                table: "document",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
