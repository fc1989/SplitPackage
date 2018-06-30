using Abp.Authorization.Users;
using Abp.Events.Bus;
using Abp.Events.Bus.Entities;
using SplitPackage.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace SplitPackage.Tests.Contexts
{
    public class AssistantCase : BaseCaseContext
    {
        public AssistantCase():base()
        {
            LoginAsTenant("AstraeaAssistant", AbpUserBase.AdminUserName);
        }

        public override void InitData(SplitPackageDbContext context)
        {
            new SkuRuleBuilder(context, context.Tenants.First(o=>o.TenancyName == "AstraeaAssistant").Id).Create();
        }
    }

    [CollectionDefinition("Assistant collection")]
    public class AssistantCaseCollection : ICollectionFixture<AssistantCase>
    {

    }

    public class SkuRuleBuilder
    {
        private readonly SplitPackageDbContext _context;
        private readonly int? tenantId;

        public SkuRuleBuilder(SplitPackageDbContext context, int? tenantId)
        {
            this._context = context;
            this.tenantId = tenantId;
        }

        public void Create()
        {
            CreateSku();
            CreateRule();
        }

        private void CreateSku()
        {
            this._context.Products.Add(new SplitPackage.Business.Product() {
                IsActive = true,
                TenantId = this.tenantId,
                PTId = "1019904",
                ProductName = "不是奶粉是保健品",
                Sku = "20180629001",
                Brand = "夯大力",
                Weight = 100,
                DeclarePrice = 100,
                DeclareTaxrate = 100
            });
            this._context.Products.Add(new SplitPackage.Business.Product()
            {
                IsActive = true,
                TenantId = this.tenantId,
                PTId = "1019904",
                ProductName = "我是10包药",
                Sku = "20180629002",
                Brand = "夯大力",
                Weight = 100,
                DeclarePrice = 100,
                DeclareTaxrate = 100
            });
            this._context.Products.Add(new SplitPackage.Business.Product()
            {
                IsActive = true,
                TenantId = this.tenantId,
                PTId = "1019904",
                ProductName = "单包零售都可以",
                Sku = "20180629003",
                Brand = "夯大力",
                Weight = 100,
                DeclarePrice = 100,
                DeclareTaxrate = 100
            });
        }

        private void CreateRule()
        {
            var tlc = this._context.TenantLogisticChannel.IgnoreQueryFilters()
                .Include(p=>p.LogisticChannelBy).ThenInclude(p=>p.SplitRules)
                .Where(o => o.TenantId == tenantId).First();
            this._context.SplitRuleProductClass.Add(new SplitPackage.Business.SplitRuleItem() {
                StintMark = "20180629002",
                SplitRuleId = tlc.LogisticChannelBy.SplitRules.First().Id,
                MinNum = 10,
                MaxNum = 10,
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                Type = SplitPackage.Business.RuleItemStintType.Sku
            });
            this._context.SplitRuleProductClass.Add(new SplitPackage.Business.SplitRuleItem()
            {
                StintMark = "20180629003",
                SplitRuleId = tlc.LogisticChannelBy.SplitRules.First().Id,
                MinNum = 1,
                MaxNum = 10,
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                Type = SplitPackage.Business.RuleItemStintType.Sku
            });
        }
    }
}
