using Microsoft.EntityFrameworkCore;
using SplitPackage.EntityFrameworkCore;
using System.Linq;
using SplitPackage.Business;

namespace SplitPackage.EntityFrameworkCore.Seed.Business
{
    public class DefaultLogisticCreator
    {
        private readonly SplitPackageDbContext _context;
        private readonly int _tenantId;

        public DefaultLogisticCreator(SplitPackageDbContext context, int tenantId)
        {
            this._context = context;
            this._tenantId = tenantId;
        }

        public void Create()
        {
            AddIfNotExists(new Logistic()
            {
                TenantId = this._tenantId,
                CorporationName = "测试物流企业",
                CorporationUrl = "测试网址:www.ceshi.com",
                LogisticFlag = "csLogistic"
            });
        }

        private void AddIfNotExists(Logistic l)
        {
            if (_context.Logistics.IgnoreQueryFilters().Any(s => s.LogisticFlag == l.LogisticFlag && s.TenantId == l.TenantId))
            {
                return;
            }

            _context.Logistics.Add(l);
            _context.SaveChanges();
        }
    }
}
