using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BorrowBooks.Migrations
{
    public partial class _2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Borrows",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsTrue",
                table: "Borrows",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Borrows");

            migrationBuilder.DropColumn(
                name: "IsTrue",
                table: "Borrows");
        }
    }
}
