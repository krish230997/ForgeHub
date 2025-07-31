using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForgeHub.Migrations
{
    /// <inheritdoc />
    public partial class ForDeletingRfq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RFQVendors_RFQs_RFQId",
                table: "RFQVendors");

            migrationBuilder.AddForeignKey(
                name: "FK_RFQVendors_RFQs_RFQId",
                table: "RFQVendors",
                column: "RFQId",
                principalTable: "RFQs",
                principalColumn: "RFQId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RFQVendors_RFQs_RFQId",
                table: "RFQVendors");

            migrationBuilder.AddForeignKey(
                name: "FK_RFQVendors_RFQs_RFQId",
                table: "RFQVendors",
                column: "RFQId",
                principalTable: "RFQs",
                principalColumn: "RFQId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
