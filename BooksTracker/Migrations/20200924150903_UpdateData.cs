using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BooksTracker.Migrations
{
    public partial class UpdateData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "book",
                keyColumn: "ID",
                keyValue: -3,
                column: "ReturnedDate",
                value: new DateTime(2020, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "book",
                keyColumn: "ID",
                keyValue: -1,
                columns: new[] { "DueDate", "ReturnedDate" },
                values: new object[] { new DateTime(2020, 1, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "book",
                keyColumn: "ID",
                keyValue: -3,
                column: "ReturnedDate",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified).AddTicks(28));

            migrationBuilder.UpdateData(
                table: "book",
                keyColumn: "ID",
                keyValue: -1,
                columns: new[] { "DueDate", "ReturnedDate" },
                values: new object[] { new DateTime(2020, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified).AddTicks(28) });
        }
    }
}
