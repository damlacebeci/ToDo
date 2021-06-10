using Microsoft.EntityFrameworkCore.Migrations;

namespace ToDo.Data.Migrations
{
    public partial class identityExtended : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CetUserId",
                table: "todoItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_todoItems_CetUserId",
                table: "todoItems",
                column: "CetUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_todoItems_AspNetUsers_CetUserId",
                table: "todoItems",
                column: "CetUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_todoItems_AspNetUsers_CetUserId",
                table: "todoItems");

            migrationBuilder.DropIndex(
                name: "IX_todoItems_CetUserId",
                table: "todoItems");

            migrationBuilder.DropColumn(
                name: "CetUserId",
                table: "todoItems");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "AspNetUsers");
        }
    }
}
