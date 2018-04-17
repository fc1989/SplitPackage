using Microsoft.EntityFrameworkCore;
using SplitPackage.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SplitPackage.EntityFrameworkCore.Seed.Business
{
    public class DefaultProductCreator
    {
        private readonly SplitPackageDbContext _context;
        private readonly int _tenantId;

        public DefaultProductCreator(SplitPackageDbContext context, int tenantId)
        {
            this._context = context;
            this._tenantId = tenantId;
        }

        public void Create()
        {
            AddIfNotExists(new Product() {
                TenantId = this._tenantId,
                ProductName = "测试商品",
                AbbreName = "测试",
                ProductNo = "1000000000",
                Sku = "1000000000",
                TaxNo = "123456789",
                Brand = "测试品牌",
                Weight = 10
            });
        }

        private void AddIfNotExists(Product p)
        {
            if (_context.Products.IgnoreQueryFilters().Any(s => p.Sku == p.Sku && s.TenantId == p.TenantId))
            {
                return;
            }

            _context.Products.Add(p);
            _context.SaveChanges();
        }
    }
}
