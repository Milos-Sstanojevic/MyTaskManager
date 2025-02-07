using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IvanaP.Migrations
{
    /// <inheritdoc />
    public partial class V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Salt = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ToDoTasks",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTaskStarted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTaskShouldEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaskName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateOfTask = table.Column<int>(type: "int", nullable: false),
                    OwnerOfTaskID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoTasks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ToDoTasks_Users_OwnerOfTaskID",
                        column: x => x.OwnerOfTaskID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberID = table.Column<int>(type: "int", nullable: false),
                    TaskID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Members_ToDoTasks_TaskID",
                        column: x => x.TaskID,
                        principalTable: "ToDoTasks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Members_Users_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Members_MemberID",
                table: "Members",
                column: "MemberID");

            migrationBuilder.CreateIndex(
                name: "IX_Members_TaskID",
                table: "Members",
                column: "TaskID");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoTasks_OwnerOfTaskID",
                table: "ToDoTasks",
                column: "OwnerOfTaskID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "ToDoTasks");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
