using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ATEK.Data.Migrations
{
    public partial class AddTimeChecksTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimeChecks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pinno = table.Column<string>(nullable: true),
                    GateFirebaseId = table.Column<string>(nullable: true),
                    Timecheck = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeChecks", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeChecks");
        }
    }
}
