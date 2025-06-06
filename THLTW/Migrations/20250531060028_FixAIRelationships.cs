using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace THLTW.Migrations
{
    /// <inheritdoc />
    public partial class FixAIRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserActivities_Products_ProductId",
                table: "UserActivities");

            migrationBuilder.AddForeignKey(
                name: "FK_UserActivities_Products_ProductId",
                table: "UserActivities",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserActivities_Products_ProductId",
                table: "UserActivities");

            migrationBuilder.AddForeignKey(
                name: "FK_UserActivities_Products_ProductId",
                table: "UserActivities",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
