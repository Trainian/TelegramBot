using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Telegram.Migrations
{
    public partial class ProblemAnswers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "TelegramId",
                table: "Users",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<byte>(
                name: "Position",
                table: "Users",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "TelegramName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserGetProblemId",
                table: "Problems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Img",
                table: "Answers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Problems_UserGetProblemId",
                table: "Problems",
                column: "UserGetProblemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_Users_UserGetProblemId",
                table: "Problems",
                column: "UserGetProblemId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Problems_Users_UserGetProblemId",
                table: "Problems");

            migrationBuilder.DropIndex(
                name: "IX_Problems_UserGetProblemId",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TelegramName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserGetProblemId",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "Img",
                table: "Answers");

            migrationBuilder.AlterColumn<int>(
                name: "TelegramId",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
