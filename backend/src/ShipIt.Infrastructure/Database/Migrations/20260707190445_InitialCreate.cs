using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipIt.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "delivery_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    order_number = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    sender_city = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    sender_street = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    sender_house = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    sender_apartment = table.Column<int>(type: "INTEGER", nullable: true),
                    recipient_city = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    recipient_street = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    recipient_house = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    recipient_apartment = table.Column<int>(type: "INTEGER", nullable: true),
                    cargo_weight = table.Column<decimal>(type: "TEXT", precision: 10, scale: 3, nullable: false),
                    pickup_date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delivery_orders", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_delivery_orders_order_number",
                table: "delivery_orders",
                column: "order_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "delivery_orders");
        }
    }
}
