using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Chronicle.Web.Migrations
{
    public partial class PostTimeDeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TimeDeleted",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeDeleted",
                table: "Posts");
        }
    }
}
