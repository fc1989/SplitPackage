using Microsoft.EntityFrameworkCore;
using SplitPackage.EntityFrameworkCore;
using System.Linq;
using SplitPackage.Business;

namespace SplitPackage.EntityFrameworkCore.Seed.Business
{
    public class DefaultSplitRuleCreator
    {
        private readonly SplitPackageDbContext _context;
        private readonly long _logisticLineId;

        public DefaultSplitRuleCreator(SplitPackageDbContext context, long logisticLineId)
        {
            this._context = context;
            this._logisticLineId = logisticLineId;
        }

        public void Create()
        {
            var sr = new SplitRule()
            {
                MaxPackage = 10,
                MaxWeight = 2000,
                MaxTax = 2000,
                MaxPrice = 2000,
                LogisticLineId = this._logisticLineId
            };
            _context.SplitRules.Add(sr);
            _context.SaveChanges();
        }
    }
}
