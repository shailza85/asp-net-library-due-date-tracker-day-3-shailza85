using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BooksTracker.Migrations
{
    public partial class UpdateBookTableData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExtensionCount",
                table: "book",
                type: "int(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "book",
                keyColumn: "ID",
                keyValue: -5,
                column: "ExtensionCount",
                value: 2);

            migrationBuilder.UpdateData(
                table: "book",
                keyColumn: "ID",
                keyValue: -3,
                columns: new[] { "ExtensionCount", "ReturnedDate" },
                values: new object[] { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified).AddTicks(28) });

            migrationBuilder.UpdateData(
                table: "book",
                keyColumn: "ID",
                keyValue: -1,
                columns: new[] { "ExtensionCount", "ReturnedDate" },
                values: new object[] { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified).AddTicks(28) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtensionCount",
                table: "book");

            migrationBuilder.UpdateData(
                table: "book",
                keyColumn: "ID",
                keyValue: -3,
                column: "ReturnedDate",
                value: null);

            migrationBuilder.UpdateData(
                table: "book",
                keyColumn: "ID",
                keyValue: -1,
                column: "ReturnedDate",
                value: null);
        }
    }
}
