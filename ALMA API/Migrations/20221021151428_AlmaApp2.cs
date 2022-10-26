using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ALMA_API.Migrations
{
    public partial class AlmaApp2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManageProduction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FarmId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManageProduction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManageProduction_Farm_FarmId",
                        column: x => x.FarmId,
                        principalTable: "Farm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Stall",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Side = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ManageProductionId = table.Column<int>(type: "int", nullable: false),
                    CowId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stall", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stall_Cow_CowId",
                        column: x => x.CowId,
                        principalTable: "Cow",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Stall_ManageProduction_ManageProductionId",
                        column: x => x.ManageProductionId,
                        principalTable: "ManageProduction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ManageProduction_FarmId",
                table: "ManageProduction",
                column: "FarmId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stall_CowId",
                table: "Stall",
                column: "CowId");

            migrationBuilder.CreateIndex(
                name: "IX_Stall_ManageProductionId",
                table: "Stall",
                column: "ManageProductionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stall");

            migrationBuilder.DropTable(
                name: "ManageProduction");
        }
    }
}
