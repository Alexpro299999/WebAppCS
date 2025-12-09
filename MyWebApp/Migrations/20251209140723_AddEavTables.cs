using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddEavTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EavEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EavEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EavAttributes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DataType = table.Column<string>(type: "TEXT", nullable: false),
                    EavEntityId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EavAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EavAttributes_EavEntities_EavEntityId",
                        column: x => x.EavEntityId,
                        principalTable: "EavEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EavRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EavEntityId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EavRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EavRecords_EavEntities_EavEntityId",
                        column: x => x.EavEntityId,
                        principalTable: "EavEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EavValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    EavRecordId = table.Column<int>(type: "INTEGER", nullable: false),
                    EavAttributeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EavValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EavValues_EavAttributes_EavAttributeId",
                        column: x => x.EavAttributeId,
                        principalTable: "EavAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EavValues_EavRecords_EavRecordId",
                        column: x => x.EavRecordId,
                        principalTable: "EavRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EavAttributes_EavEntityId",
                table: "EavAttributes",
                column: "EavEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EavRecords_EavEntityId",
                table: "EavRecords",
                column: "EavEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EavValues_EavAttributeId",
                table: "EavValues",
                column: "EavAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_EavValues_EavRecordId",
                table: "EavValues",
                column: "EavRecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EavValues");

            migrationBuilder.DropTable(
                name: "EavAttributes");

            migrationBuilder.DropTable(
                name: "EavRecords");

            migrationBuilder.DropTable(
                name: "EavEntities");
        }
    }
}
