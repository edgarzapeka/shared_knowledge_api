using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SharedKnowledgeAPI.Migrations
{
    public partial class dsa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Link");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Link",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Link_UserId",
                table: "Link",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Link_AspNetUsers_UserId",
                table: "Link",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Link_AspNetUsers_UserId",
                table: "Link");

            migrationBuilder.DropIndex(
                name: "IX_Link_UserId",
                table: "Link");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Link",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Link",
                nullable: true);
        }
    }
}
