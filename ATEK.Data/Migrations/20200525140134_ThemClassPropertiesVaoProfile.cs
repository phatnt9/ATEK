using Microsoft.EntityFrameworkCore.Migrations;

namespace ATEK.Data.Migrations
{
    public partial class ThemClassPropertiesVaoProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Class",
                table: "Profiles");

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "Profiles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_ClassId",
                table: "Profiles",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_Classes_ClassId",
                table: "Profiles",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_Classes_ClassId",
                table: "Profiles");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Profiles_ClassId",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Profiles");

            migrationBuilder.AddColumn<string>(
                name: "Class",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
