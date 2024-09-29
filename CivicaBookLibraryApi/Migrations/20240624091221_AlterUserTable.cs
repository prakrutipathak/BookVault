using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CivicaBookLibraryApi.Migrations
{
    public partial class AlterUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PasswordHint",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "SecurityQuestions",
                columns: table => new
                {
                    PasswordHint = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityQuestions", x => x.PasswordHint);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_PasswordHint",
                table: "Users",
                column: "PasswordHint");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_SecurityQuestions_PasswordHint",
                table: "Users",
                column: "PasswordHint",
                principalTable: "SecurityQuestions",
                principalColumn: "PasswordHint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_SecurityQuestions_PasswordHint",
                table: "Users");

            migrationBuilder.DropTable(
                name: "SecurityQuestions");

            migrationBuilder.DropIndex(
                name: "IX_Users_PasswordHint",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHint",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
