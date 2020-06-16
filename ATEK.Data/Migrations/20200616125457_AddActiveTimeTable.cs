using Microsoft.EntityFrameworkCore.Migrations;

namespace ATEK.Data.Migrations
{
    public partial class AddActiveTimeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActiveTime",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromTime = table.Column<string>(nullable: true),
                    ToTime = table.Column<string>(nullable: true),
                    ProfileGateProfileId = table.Column<int>(nullable: false),
                    ProfileGateGateId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveTime", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActiveTime_ProfileGate_ProfileGateProfileId_ProfileGateGateId",
                        columns: x => new { x.ProfileGateProfileId, x.ProfileGateGateId },
                        principalTable: "ProfileGate",
                        principalColumns: new[] { "ProfileId", "GateId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveTime_ProfileGateProfileId_ProfileGateGateId",
                table: "ActiveTime",
                columns: new[] { "ProfileGateProfileId", "ProfileGateGateId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveTime");
        }
    }
}