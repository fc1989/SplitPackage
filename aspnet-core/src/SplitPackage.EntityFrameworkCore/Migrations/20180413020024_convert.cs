using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using SplitPackage.Business;
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
                    ProductName = table.Column<string>(nullable: false, maxLength : Product.MaxProductNameLength),
                    AbbreName = table.Column<string>(nullable: true, maxLength: Product.MaxAbbreNameLength),
                    ProductNo = table.Column<string>(nullable: false, maxLength: Product.MaxProductNoLength),
                    Sku = table.Column<string>(nullable: false, maxLength: Product.MaxSkuLength),
                    TaxNo = table.Column<string>(nullable:true, maxLength: Product.MaxTaxNoLength),
                    Brand = table.Column<string>(nullable:true, maxLength: Product.MaxBrandLength),
                    Weight = table.Column<double>(defaultValue: Product.DefaultWeightValue),
                    TenantId = table.Column<int>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Users_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Users_DeleterUserId",
                        column: x => x.DeleterUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Users_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.UniqueConstraint(
                        name: "UQ_Products",
                        columns:x=>new { x.TenantId, x.Sku});
                });

            migrationBuilder.CreateTable(
                name: "Logistics",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CorporationName = table.Column<string>(maxLength:Logistic.MaxCorporationNameLength),
                    CorporationUrl = table.Column<string>(nullable:true, maxLength: Logistic.MaxCorporationUrlLength),
                    LogisticFlag = table.Column<string>(maxLength :Logistic.MaxLogisticFlagLength),
                    TenantId = table.Column<int>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logistics_Users_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Logistics_Users_DeleterUserId",
                        column: x => x.DeleterUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Logistics_Users_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Logistics_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.UniqueConstraint(name:"UQ_Logistics",columns:x=>new { x.TenantId, x.LogisticFlag});
                });

            migrationBuilder.CreateTable(
                name: "LogisticLines",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LineName = table.Column<string>(maxLength: LogisticLine.MaxLineNameLength),
                    LineCode = table.Column<string>(maxLength: LogisticLine.MaxLineCodeLength),
                    LogisticId = table.Column<long>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(defaultValue: true),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogisticLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogisticLines_Users_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LogisticLines_Users_DeleterUserId",
                        column: x => x.DeleterUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LogisticLines_Users_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LogisticLines_Logistics_LogisticId",
                        column: x => x.LogisticId,
                        principalTable: "Logistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LogisticLines_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.UniqueConstraint(name: "UQ_LogisticLines", columns: x => new { x.LogisticId, x.LineCode });
                });

            migrationBuilder.CreateTable(
                name: "NumFreights",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductNum = table.Column<int>(),
                    PackagePrice = table.Column<double>(),
                    LogisticLineId = table.Column<long>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(defaultValue: true),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumFreights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NumFreights_Users_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NumFreights_Users_DeleterUserId",
                        column: x => x.DeleterUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NumFreights_Users_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NumFreights_LogisticLines_LogisticLineId",
                        column: x => x.LogisticLineId,
                        principalTable: "LogisticLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NumFreights_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductClasses",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClassName = table.Column<string>(maxLength: ProductClass.MaxClassNameLength),
                    PTId = table.Column<string>(maxLength: ProductClass.MaxPTIdLength),
                    PostTaxRate = table.Column<double>(),
                    BCTaxRate = table.Column<double>(),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(defaultValue: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductClasses_Users_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductClasses_Users_DeleterUserId",
                        column: x => x.DeleterUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductClasses_Users_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.UniqueConstraint(name: "UQ_ProductClasses", columns: x => new { x.PTId });
                });

            migrationBuilder.CreateTable(
                name: "SplitRules",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MaxPackage = table.Column<int>(),
                    MaxWeight = table.Column<double>(),
                    MaxTax = table.Column<double>(),
                    MaxPrice = table.Column<double>(),
                    LogisticLineId = table.Column<long>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(defaultValue: true),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SplitRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SplitRules_Users_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SplitRules_Users_DeleterUserId",
                        column: x => x.DeleterUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SplitRules_Users_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SplitRules_LogisticLines_LogisticLineId",
                        column: x => x.LogisticLineId,
                        principalTable: "LogisticLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SplitRules_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SplitRule_ProductClass",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductClassId = table.Column<long>(),
                    SplitRuleId = table.Column<long>(),
                    MaxNum = table.Column<int>(),
                    MinNum = table.Column<int>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SplitRuleProductClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SplitRuleProductClass_ProductClasses_ProductClassId",
                        column: x => x.ProductClassId,
                        principalTable: "ProductClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SplitRuleProductClass_SplitRules_SplitRuleId",
                        column: x => x.SplitRuleId,
                        principalTable: "SplitRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WeightFreights",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StartingWeight = table.Column<double>(),
                    StartingPrice = table.Column<double>(),
                    StepWeight = table.Column<double>(),
                    Price = table.Column<double>(),
                    LogisticLineId = table.Column<long>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(defaultValue: true),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeightFreights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeightFreights_Users_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WeightFreights_Users_DeleterUserId",
                        column: x => x.DeleterUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WeightFreights_Users_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WeightFreights_LogisticLines_LogisticLineId",
                        column: x => x.LogisticLineId,
                        principalTable: "LogisticLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WeightFreights_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Product_ProductClass",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<long>(nullable:false),
                    ProductClassId = table.Column<long>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductProductClass",x=>x.Id);
                    table.ForeignKey(
                        name: "FK_ProductProductClass_ProductClasses_ProductClassId",
                        column: x => x.ProductClassId,
                        principalTable: "ProductClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductProductClass_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.UniqueConstraint("UQ_ProductProductClass", x => new { x.ProductClassId, x.ProductId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreatorUserId",
                table: "Products",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_DeleterUserId",
                table: "Products",
                column: "DeleterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_LastModifierUserId",
                table: "Products",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId",
                table: "Products",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Logistics_CreatorUserId",
                table: "Logistics",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Logistics_DeleterUserId",
                table: "Logistics",
                column: "DeleterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Logistics_LastModifierUserId",
                table: "Logistics",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Logistics_TenantId",
                table: "Logistics",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LogisticLines_CreatorUserId",
                table: "LogisticLines",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LogisticLines_DeleterUserId",
                table: "LogisticLines",
                column: "DeleterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LogisticLines_LastModifierUserId",
                table: "LogisticLines",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LogisticLines_LogisticId",
                table: "LogisticLines",
                column: "LogisticId");

            migrationBuilder.CreateIndex(
                name: "IX_LogisticLines_TenantId",
                table: "LogisticLines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_NumFreights_CreatorUserId",
                table: "NumFreights",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_NumFreights_DeleterUserId",
                table: "NumFreights",
                column: "DeleterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_NumFreights_LastModifierUserId",
                table: "NumFreights",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_NumFreights_LogisticLineId",
                table: "NumFreights",
                column: "LogisticLineId");

            migrationBuilder.CreateIndex(
                name: "IX_NumFreights_TenantId",
                table: "NumFreights",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductClasses_CreatorUserId",
                table: "ProductClasses",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductClasses_DeleterUserId",
                table: "ProductClasses",
                column: "DeleterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductClasses_LastModifierUserId",
                table: "ProductClasses",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SplitRules_CreatorUserId",
                table: "SplitRules",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SplitRules_DeleterUserId",
                table: "SplitRules",
                column: "DeleterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SplitRules_LastModifierUserId",
                table: "SplitRules",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SplitRules_LogisticLineId",
                table: "SplitRules",
                column: "LogisticLineId");

            migrationBuilder.CreateIndex(
                name: "IX_SplitRules_TenantId",
                table: "SplitRules",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SplitRuleProductClass_ProductClassId",
                table: "SplitRule_ProductClass",
                column: "ProductClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SplitRuleProductClass_SplitRuleId",
                table: "SplitRule_ProductClass",
                column: "SplitRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_WeightFreights_CreatorUserId",
                table: "WeightFreights",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WeightFreights_DeleterUserId",
                table: "WeightFreights",
                column: "DeleterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WeightFreights_LastModifierUserId",
                table: "WeightFreights",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WeightFreights_LogisticLineId",
                table: "WeightFreights",
                column: "LogisticLineId");

            migrationBuilder.CreateIndex(
                name: "IX_WeightFreights_TenantId",
                table: "WeightFreights",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductProductClass_ProductClassId",
                table: "Product_ProductClass",
                column: "ProductClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductProductClass_ProductId",
                table: "Product_ProductClass",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "SplitRule_ProductClass");
            migrationBuilder.DropTable(name: "Product_ProductClass");
            migrationBuilder.DropTable(name: "SplitRules");
            migrationBuilder.DropTable(name: "NumFreights");
            migrationBuilder.DropTable(name: "WeightFreights");
            migrationBuilder.DropTable(name: "ProductClasses");
            migrationBuilder.DropTable(name: "Products");
            migrationBuilder.DropTable(name: "LogisticLines");
            migrationBuilder.DropTable(name: "Logistics");
        }
    }
}
