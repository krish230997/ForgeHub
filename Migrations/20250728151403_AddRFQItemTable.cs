using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForgeHub.Migrations
{
    /// <inheritdoc />
    public partial class AddRFQItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RFQItem",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RFQId = table.Column<int>(type: "int", nullable: false),
                    RFQLineNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReqQty = table.Column<int>(type: "int", nullable: false),
                    UOM = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReqDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FactoryCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RFQItem", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_RFQItem_RFQs_RFQId",
                        column: x => x.RFQId,
                        principalTable: "RFQs",
                        principalColumn: "RFQId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RFQItem_RFQId",
                table: "RFQItem",
                column: "RFQId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RFQItem");
        }
    }
}
