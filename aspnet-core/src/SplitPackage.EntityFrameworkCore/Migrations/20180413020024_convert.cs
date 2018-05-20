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
                    ProductName = table.Column<string>(nullable: false, maxLength: Product.MaxProductNameLength),
                    Sku = table.Column<string>(nullable: false, maxLength: Product.MaxSkuLength),
                    Brand = table.Column<string>(nullable: true, maxLength: Product.MaxBrandLength),
                    Weight = table.Column<double>(defaultValue: Product.DefaultWeightValue),
                    DeclarePrice = table.Column<double>(defaultValue: Product.DefaultDeclarePriceValue),
                    DeclareTaxrate = table.Column<double>(defaultValue: Product.DefaultDeclareTaxrateValue),
                    TenantId = table.Column<int>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    PTId = table.Column<string>(nullable: false, maxLength: Product.MaxPTIdLength)
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
                        columns: x => new { x.TenantId, x.Sku });
                });

            migrationBuilder.CreateTable(
                name: "Logistics",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CorporationName = table.Column<string>(maxLength: Logistic.MaxCorporationNameLength),
                    CorporationUrl = table.Column<string>(nullable: true, maxLength: Logistic.MaxCorporationUrlLength),
                    LogisticCode = table.Column<string>(maxLength: Logistic.MaxLogisticCodeLength),
                    TenantId = table.Column<int>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
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
                    table.UniqueConstraint(name: "UQ_Logistics", columns: x => new { x.TenantId, x.LogisticCode });
                });

            migrationBuilder.CreateTable(
                name: "LogisticChannels",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ChannelName = table.Column<string>(maxLength: LogisticChannel.MaxChannelNameLength),
                    AliasName = table.Column<string>(maxLength: LogisticChannel.MaxAliasNameLength, nullable: true),
                    Type = table.Column<ChannelType>(nullable: false),
                    Way = table.Column<ChargeWay>(nullable: false),
                    LogisticId = table.Column<long>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogisticChannels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogisticChannels_Users_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LogisticChannels_Users_DeleterUserId",
                        column: x => x.DeleterUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LogisticChannels_Users_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LogisticChannels_Logistics_LogisticId",
                        column: x => x.LogisticId,
                        principalTable: "Logistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LogisticChannels_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.UniqueConstraint(name: "UQ_LogisticChannels", columns: x => new { x.LogisticId, x.ChannelName });
                });

            migrationBuilder.CreateTable(
                name: "NumFreights",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Currency = table.Column<string>(nullable: true, maxLength: CommonConstraintConst.MaxCurrencyLength),
                    Unit = table.Column<string>(nullable: true, maxLength: CommonConstraintConst.MaxUnitLength),
                    SplitNum = table.Column<double>(nullable: false),
                    FirstPrice = table.Column<double>(nullable: false),
                    CarryOnPrice = table.Column<double>(nullable: false),
                    LogisticChannelId = table.Column<long>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
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
                        name: "FK_NumFreights_LogisticChannels_LogisticChannelId",
                        column: x => x.LogisticChannelId,
                        principalTable: "LogisticChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SplitRules",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RuleName = table.Column<string>(nullable: true, maxLength: SplitRule.MaxRuleNameLength),
                    MaxPackage = table.Column<int>(),
                    MaxWeight = table.Column<double>(),
                    MaxTax = table.Column<double>(),
                    MaxPrice = table.Column<double>(),
                    LogisticChannelId = table.Column<long>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
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
                        name: "FK_SplitRules_LogisticChannels_LogisticChannelId",
                        column: x => x.LogisticChannelId,
                        principalTable: "LogisticChannels",
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
                    PTId = table.Column<string>(nullable: false, maxLength: Product.MaxPTIdLength),
                    SplitRuleId = table.Column<long>(nullable: false),
                    MaxNum = table.Column<int>(),
                    MinNum = table.Column<int>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SplitRuleProductClass", x => x.Id);
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
                    Currency = table.Column<string>(maxLength: CommonConstraintConst.MaxCurrencyLength),
                    Unit = table.Column<string>(maxLength: CommonConstraintConst.MaxUnitLength),
                    StartingWeight = table.Column<double>(),
                    EndWeight = table.Column<double>(),
                    StartingPrice = table.Column<double>(),
                    StepWeight = table.Column<double>(),
                    Price = table.Column<double>(),
                    CostPrice = table.Column<double>(),
                    LogisticChannelId = table.Column<long>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
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
                        name: "FK_WeightFreights_LogisticChannels_LogisticChannelId",
                        column: x => x.LogisticChannelId,
                        principalTable: "LogisticChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tenant_LogisticChannel",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    LogisticChannelId = table.Column<long>(nullable: false),
                    LogisticChannelChange = table.Column<string>(type: "text", nullable: true),
                    AliasName = table.Column<string>(nullable: true, maxLength: LogisticChannel.MaxAliasNameLength),
                    Way = table.Column<ChargeWay>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantLogisticChannel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantLogisticChannel_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantLogisticChannel_LogisticChannels_LogisticChannelId",
                        column: x => x.LogisticChannelId,
                        principalTable: "LogisticChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogisticRelateds",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: true),
                    RelatedName = table.Column<string>(nullable:false, maxLength:LogisticRelated.MaxRelatedNameLength)
                },
                constraints: table => {
                    table.PrimaryKey("PK_LogisticRelateds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogisticRelateds_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogisticRelatedItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LogisticRelatedId = table.Column<long>(nullable:false),
                    LogisticId = table.Column<long>(nullable:false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_LogisticRelatedItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogisticRelatedItems_LogisticRelateds_LogisticRelatedId",
                        column: x => x.LogisticRelatedId,
                        principalTable: "LogisticRelateds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LogisticRelatedItems_Logistics_LogisticId",
                        column: x => x.LogisticId,
                        principalTable: "Logistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductSorts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SortName = table.Column<string>(nullable: false, maxLength: ProductSort.MaxSortNameLength),
                    IsActive = table.Column<bool>(nullable:false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_LogisticRelatedItems", x => x.Id);
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
                    ProductSortId = table.Column<long>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductClasses_ProductSorts_ProductSortId",
                        column: x => x.ProductSortId,
                        principalTable: "ProductSorts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductClasses_ProductSortId",
                table: "ProductClasses",
                column: "ProductSortId");

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
                name: "IX_LogisticChannels_CreatorUserId",
                table: "LogisticChannels",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LogisticChannels_DeleterUserId",
                table: "LogisticChannels",
                column: "DeleterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LogisticChannels_LastModifierUserId",
                table: "LogisticChannels",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LogisticChannels_LogisticId",
                table: "LogisticChannels",
                column: "LogisticId");

            migrationBuilder.CreateIndex(
                name: "IX_LogisticChannels_TenantId",
                table: "LogisticChannels",
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
                name: "IX_NumFreights_LogisticChannelId",
                table: "NumFreights",
                column: "LogisticChannelId");

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
                name: "IX_SplitRules_LogisticChannelId",
                table: "SplitRules",
                column: "LogisticChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_SplitRules_TenantId",
                table: "SplitRules",
                column: "TenantId");

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
                name: "IX_WeightFreights_LogisticChannelId",
                table: "WeightFreights",
                column: "LogisticChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_LogisticRelateds_TenantId",
                table: "LogisticRelateds",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LogisticRelatedItems_LogisticRelatedId",
                table: "LogisticRelatedItems",
                column: "LogisticRelatedId");

            migrationBuilder.CreateIndex(
                name: "IX_LogisticRelatedItems_LogisticId",
                table: "LogisticRelatedItems",
                column: "LogisticId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "SplitRule_ProductClass");
            migrationBuilder.DropTable(name: "Tenant_LogisticChannel");
            migrationBuilder.DropTable(name: "SplitRules");
            migrationBuilder.DropTable(name: "NumFreights");
            migrationBuilder.DropTable(name: "WeightFreights");
            migrationBuilder.DropTable(name: "Products");
            migrationBuilder.DropTable(name: "LogisticChannels");
            migrationBuilder.DropTable(name: "Logistics");
            migrationBuilder.DropTable(name: "LogisticRelateds");
            migrationBuilder.DropTable(name: "LogisticRelatedItems");
        }
    }
}
