using Microsoft.EntityFrameworkCore;
using SplitPackage.EntityFrameworkCore;
using System.Linq;
using SplitPackage.Business;

namespace SplitPackage.EntityFrameworkCore.Seed.Business
{
    public class DefaultSplitRuleProductClassCreator
    {
        private readonly SplitPackageDbContext _context;

        public DefaultSplitRuleProductClassCreator(SplitPackageDbContext context)
        {
            this._context = context;
        }

        public void Create()
        {
            AddIfNotExists(new SplitRuleProductClass()
            {
                SplitRuleId = 1,
                ProductClassId = 1
            });
        }

        private void AddIfNotExists(SplitRuleProductClass spc)
        {
            if (_context.SplitRuleProductClass.IgnoreQueryFilters().Any(s => s.SplitRuleId == spc.SplitRuleId && s.ProductClassId == spc.ProductClassId))
            {
                return;
            }

            _context.SplitRuleProductClass.Add(spc);
            _context.SaveChanges();
        }
    }
}
