using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SplitPackage.Migrations
{
    public partial class task2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PTId",
                table: "SplitRule_ProductClass",
                newName: "StintMark");

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "SplitRules",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "SplitRule_ProductClass",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "SplitRule_ProductClass",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "SplitRule_ProductClass",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SplitRules");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "SplitRule_ProductClass");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SplitRule_ProductClass");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "SplitRule_ProductClass");

            migrationBuilder.RenameColumn(
                name: "StintMark",
                table: "SplitRule_ProductClass",
                newName: "PTId");
        }
    }
}
