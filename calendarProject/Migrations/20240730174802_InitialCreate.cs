using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace calendarProject.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "document",
                columns: table => new
                {
                    docId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    docName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    docDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    docStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document", x => x.docId);
                });

            migrationBuilder.CreateTable(
                name: "subjectWeek",
                columns: table => new
                {
                    subId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    stratDateWeek = table.Column<DateTime>(type: "datetime2", nullable: false),
                    lastDateWeek = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subjectWeek", x => x.subId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "document");

            migrationBuilder.DropTable(
                name: "subjectWeek");
        }
    }
}
