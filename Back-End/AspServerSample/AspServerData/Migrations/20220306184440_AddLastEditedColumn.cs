using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspServerData.Migrations
{
    public partial class AddLastEditedColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastEdited",
                table: "Posts",
                type: "datetime2(0)",
                precision: 0,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEdited",
                table: "Comments",
                type: "datetime2(0)",
                precision: 0,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastEdited",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "LastEdited",
                table: "Comments");
        }
    }
}
