using Microsoft.EntityFrameworkCore.Migrations;

namespace Chronicle.Web.Migrations
{
    public partial class CreatedTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Author_Chronicle_ChronicleId",
                table: "Author");

            migrationBuilder.DropForeignKey(
                name: "FK_Author_Users_UserId",
                table: "Author");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chronicle",
                table: "Chronicle");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Author",
                table: "Author");

            migrationBuilder.RenameTable(
                name: "Chronicle",
                newName: "Chronicles");

            migrationBuilder.RenameTable(
                name: "Author",
                newName: "Authors");

            migrationBuilder.RenameIndex(
                name: "IX_Author_UserId",
                table: "Authors",
                newName: "IX_Authors_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Author_ChronicleId",
                table: "Authors",
                newName: "IX_Authors_ChronicleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chronicles",
                table: "Chronicles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Authors",
                table: "Authors",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_Chronicles_ChronicleId",
                table: "Authors",
                column: "ChronicleId",
                principalTable: "Chronicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_Users_UserId",
                table: "Authors",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authors_Chronicles_ChronicleId",
                table: "Authors");

            migrationBuilder.DropForeignKey(
                name: "FK_Authors_Users_UserId",
                table: "Authors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chronicles",
                table: "Chronicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Authors",
                table: "Authors");

            migrationBuilder.RenameTable(
                name: "Chronicles",
                newName: "Chronicle");

            migrationBuilder.RenameTable(
                name: "Authors",
                newName: "Author");

            migrationBuilder.RenameIndex(
                name: "IX_Authors_UserId",
                table: "Author",
                newName: "IX_Author_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Authors_ChronicleId",
                table: "Author",
                newName: "IX_Author_ChronicleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chronicle",
                table: "Chronicle",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Author",
                table: "Author",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Author_Chronicle_ChronicleId",
                table: "Author",
                column: "ChronicleId",
                principalTable: "Chronicle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Author_Users_UserId",
                table: "Author",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
