using Microsoft.EntityFrameworkCore;
using SplitPackage.EntityFrameworkCore;
using System.Linq;
using SplitPackage.Business;

namespace SplitPackage.EntityFrameworkCore.Seed.Business
{
    public class DefaultProductClassCreator
    {
        private readonly SplitPackageDbContext _context;

        public DefaultProductClassCreator(SplitPackageDbContext context)
        {
            this._context = context;
        }

        public void Create()
        {
            AddIfNotExists(new ProductClass()
            {
                ClassName = "品类1",
                PTId = "0001",
                PostTaxRate = 0.6,
                BCTaxRate = 0.9
            });
        }

        private void AddIfNotExists(ProductClass pc)
        {
            if (_context.ProductClasses.IgnoreQueryFilters().Any(s => s.PTId == pc.PTId))
            {
                return;
            }

            _context.ProductClasses.Add(pc);
            _context.SaveChanges();
        }
    }
}
