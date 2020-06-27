using Microsoft.EntityFrameworkCore.Migrations;

namespace ATEK.Data.Migrations
{
    public partial class AddFirebaseIdToTimeChecksTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirebaseId",
                table: "TimeChecks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirebaseId",
                table: "TimeChecks");
        }
    }
}
