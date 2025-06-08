using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabSlides.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slides_Presentations_PresentationId",
                table: "Slides");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Presentations_PresentationId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "TextBlocks");

            migrationBuilder.DropIndex(
                name: "IX_Users_PresentationId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PresentationId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.AlterColumn<Guid>(
                name: "PresentationId",
                table: "Slides",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Slides",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PresentationUsers",
                columns: table => new
                {
                    PresentationId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentationUsers", x => new { x.PresentationId, x.UserId });
                    table.ForeignKey(
                        name: "FK_PresentationUsers_Presentations_PresentationId",
                        column: x => x.PresentationId,
                        principalTable: "Presentations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PresentationUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Presentations_CreatorId",
                table: "Presentations",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_PresentationUsers_UserId",
                table: "PresentationUsers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Presentations_Users_CreatorId",
                table: "Presentations",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Slides_Presentations_PresentationId",
                table: "Slides",
                column: "PresentationId",
                principalTable: "Presentations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Presentations_Users_CreatorId",
                table: "Presentations");

            migrationBuilder.DropForeignKey(
                name: "FK_Slides_Presentations_PresentationId",
                table: "Slides");

            migrationBuilder.DropTable(
                name: "PresentationUsers");

            migrationBuilder.DropIndex(
                name: "IX_Presentations_CreatorId",
                table: "Presentations");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Slides");

            migrationBuilder.AddColumn<Guid>(
                name: "PresentationId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "PresentationId",
                table: "Slides",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "TextBlocks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    FontSize = table.Column<int>(type: "integer", nullable: false),
                    FontStyle = table.Column<string>(type: "text", nullable: false),
                    FontWeight = table.Column<string>(type: "text", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    SlideId = table.Column<Guid>(type: "uuid", nullable: true),
                    TextAlign = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TextBlocks_Slides_SlideId",
                        column: x => x.SlideId,
                        principalTable: "Slides",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_PresentationId",
                table: "Users",
                column: "PresentationId");

            migrationBuilder.CreateIndex(
                name: "IX_TextBlocks_SlideId",
                table: "TextBlocks",
                column: "SlideId");

            migrationBuilder.AddForeignKey(
                name: "FK_Slides_Presentations_PresentationId",
                table: "Slides",
                column: "PresentationId",
                principalTable: "Presentations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Presentations_PresentationId",
                table: "Users",
                column: "PresentationId",
                principalTable: "Presentations",
                principalColumn: "Id");
        }
    }
}
