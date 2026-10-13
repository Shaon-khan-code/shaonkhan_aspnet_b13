using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaleFashion.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GuestId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GuestId",
                table: "Addresses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_GuestId",
                table: "Orders",
                column: "GuestId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_GuestId",
                table: "Addresses",
                column: "GuestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_GuestId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_GuestId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "GuestId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GuestId",
                table: "Addresses");
        }
    }
}
