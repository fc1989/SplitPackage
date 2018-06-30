using SplitPackage.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;

namespace SplitPackage.Tests.Contexts
{
    /// <summary>
    /// 不允许在测试用例中进行数据修改,包括登陆状态(默认系统管理员)
    /// 建议进行管理员用例测试用例
    /// </summary>
    public class ReadStatelessCase : BaseCaseContext
    {
        public ReadStatelessCase() : base()
        {

        }

        public override void InitData(SplitPackageDbContext context)
        {
            new AddRuleBuilder(context, null).Create();
        }
    }

    [CollectionDefinition("ReadStateless collection")]
    public class ReadStatelessCollection : ICollectionFixture<ReadStatelessCase>
    {

    }

    public class AddRuleBuilder
    {
        private readonly SplitPackageDbContext _context;
        private readonly int? tenantId;

        public AddRuleBuilder(SplitPackageDbContext context, int? tenantId)
        {
            this._context = context;
            this.tenantId = tenantId;
        }

        public void Create()
        {
            AddSpecialRule();
        }

        private void AddSpecialRule()
        {
            this._context.ProductClass.Add(new SplitPackage.Business.ProductClass() {
                ClassName = "夯大力",
                PTId = "1234567890",
                ProductSortId = this._context.ProductSort.First().Id
            });
            this._context.SplitRuleProductClass.Add(new SplitPackage.Business.SplitRuleItem() {
                StintMark = "1234567890",
                SplitRuleId = this._context.SplitRules.Where(o=>o.TenantId == tenantId).First().Id,
                MinNum = 1000,
                MaxNum = 1000,
                CreationTime = DateTime.Now,
                TenantId = tenantId
            });
        }
    }
}
