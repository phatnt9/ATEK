using Microsoft.EntityFrameworkCore.Migrations;

namespace ATEK.Data.Migrations
{
    public partial class EditProfilePinnoUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Pinno",
                table: "Profiles",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_Pinno",
                table: "Profiles",
                column: "Pinno",
                unique: true,
                filter: "[Pinno] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Profiles_Pinno",
                table: "Profiles");

            migrationBuilder.AlterColumn<string>(
                name: "Pinno",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
