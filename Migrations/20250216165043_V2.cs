using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IvanaP.Migrations
{
    /// <inheritdoc />
    public partial class V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoTasks_Users_OwnerOfTaskID",
                table: "ToDoTasks");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerOfTaskID",
                table: "ToDoTasks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoTasks_Users_OwnerOfTaskID",
                table: "ToDoTasks",
                column: "OwnerOfTaskID",
                principalTable: "Users",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoTasks_Users_OwnerOfTaskID",
                table: "ToDoTasks");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerOfTaskID",
                table: "ToDoTasks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoTasks_Users_OwnerOfTaskID",
                table: "ToDoTasks",
                column: "OwnerOfTaskID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
