using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBookShelf.Migrations
{
    /// <inheritdoc />
    public partial class Opinion3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookStatus_Book_BookId",
                table: "BookStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_BookStatus_User_UserId",
                table: "BookStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookStatus",
                table: "BookStatus");

            migrationBuilder.RenameTable(
                name: "BookStatus",
                newName: "BookStatuses");

            migrationBuilder.RenameIndex(
                name: "IX_BookStatus_UserId",
                table: "BookStatuses",
                newName: "IX_BookStatuses_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookStatuses",
                table: "BookStatuses",
                columns: new[] { "BookId", "UserId" });

            migrationBuilder.CreateTable(
                name: "Opinion",
                columns: table => new
                {
                    BookId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Rank = table.Column<int>(type: "INTEGER", nullable: false),
                    Review = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opinion", x => new { x.BookId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Opinion_Book_BookId",
                        column: x => x.BookId,
                        principalTable: "Book",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Opinion_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Opinion_UserId",
                table: "Opinion",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookStatuses_Book_BookId",
                table: "BookStatuses",
                column: "BookId",
                principalTable: "Book",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookStatuses_User_UserId",
                table: "BookStatuses",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookStatuses_Book_BookId",
                table: "BookStatuses");

            migrationBuilder.DropForeignKey(
                name: "FK_BookStatuses_User_UserId",
                table: "BookStatuses");

            migrationBuilder.DropTable(
                name: "Opinion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookStatuses",
                table: "BookStatuses");

            migrationBuilder.RenameTable(
                name: "BookStatuses",
                newName: "BookStatus");

            migrationBuilder.RenameIndex(
                name: "IX_BookStatuses_UserId",
                table: "BookStatus",
                newName: "IX_BookStatus_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookStatus",
                table: "BookStatus",
                columns: new[] { "BookId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BookStatus_Book_BookId",
                table: "BookStatus",
                column: "BookId",
                principalTable: "Book",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookStatus_User_UserId",
                table: "BookStatus",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
