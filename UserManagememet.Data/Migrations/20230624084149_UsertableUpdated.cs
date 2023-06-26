using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagememet.Data.Migrations
{
    /// <inheritdoc />
    public partial class UsertableUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssignUser",
                columns: table => new
                {
                    AssignUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssignedManagerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssignedHrId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignUser", x => x.AssignUserId);
                    table.ForeignKey(
                        name: "FK_AssignUser_AspNetUsers_AssignedHrId",
                        column: x => x.AssignedHrId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssignUser_AspNetUsers_AssignedManagerId",
                        column: x => x.AssignedManagerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssignUser_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignUser_AssignedHrId",
                table: "AssignUser",
                column: "AssignedHrId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignUser_AssignedManagerId",
                table: "AssignUser",
                column: "AssignedManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignUser_UserId",
                table: "AssignUser",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignUser");
        }
    }
}
