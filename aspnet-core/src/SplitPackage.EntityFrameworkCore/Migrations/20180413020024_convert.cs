using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SplitPackage.Migrations
{
    public partial class convert : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductName = table.Column<string>(nullable: false, maxLength : 200),
                    AbbreName = table.Column<string>(nullable: false, maxLength: 100),
                    ProductNo = table.Column<string>(nullable: false, maxLength: 50),
                    Sku = table.Column<string>(nullable: false, maxLength: 50),
                    TaxNo = table.Column<string>(maxLength: 20),
                    Brand = table.Column<string>(maxLength: 50),
                    Weight = table.Column<double>(defaultValue:0d),
                    TenantId = table.Column<int>(),
                    CreationTime = table.Column<DateTime>(),
                    CreatorUserId = table.Column<long>(),
                    DeleterUserId = table.Column<long>(),
                    DeletionTime = table.Column<DateTime>(),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(defaultValue: true),
                    LastModificationTime = table.Column<DateTime>(),
                    LastModifierUserId = table.Column<long>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_Users_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Users_DeleterUserId",
                        column: x => x.DeleterUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Users_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_CreatorUserId",
                table: "Products",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_DeleterUserId",
                table: "Products",
                column: "DeleterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_LastModifierUserId",
                table: "Products",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_TenantId",
                table: "Products",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
