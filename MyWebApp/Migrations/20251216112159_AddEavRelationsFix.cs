using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddEavRelationsFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EavValues_EavAttributes_EavAttributeId",
                table: "EavValues");

            migrationBuilder.AddColumn<int>(
                name: "LinkedRecordId",
                table: "EavValues",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EavValues_LinkedRecordId",
                table: "EavValues",
                column: "LinkedRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_EavAttributes_LinkedEntityId",
                table: "EavAttributes",
                column: "LinkedEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_EavAttributes_EavEntities_LinkedEntityId",
                table: "EavAttributes",
                column: "LinkedEntityId",
                principalTable: "EavEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EavValues_EavAttributes_EavAttributeId",
                table: "EavValues",
                column: "EavAttributeId",
                principalTable: "EavAttributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EavValues_EavRecords_LinkedRecordId",
                table: "EavValues",
                column: "LinkedRecordId",
                principalTable: "EavRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EavAttributes_EavEntities_LinkedEntityId",
                table: "EavAttributes");

            migrationBuilder.DropForeignKey(
                name: "FK_EavValues_EavAttributes_EavAttributeId",
                table: "EavValues");

            migrationBuilder.DropForeignKey(
                name: "FK_EavValues_EavRecords_LinkedRecordId",
                table: "EavValues");

            migrationBuilder.DropIndex(
                name: "IX_EavValues_LinkedRecordId",
                table: "EavValues");

            migrationBuilder.DropIndex(
                name: "IX_EavAttributes_LinkedEntityId",
                table: "EavAttributes");

            migrationBuilder.DropColumn(
                name: "LinkedRecordId",
                table: "EavValues");

            migrationBuilder.AddForeignKey(
                name: "FK_EavValues_EavAttributes_EavAttributeId",
                table: "EavValues",
                column: "EavAttributeId",
                principalTable: "EavAttributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
