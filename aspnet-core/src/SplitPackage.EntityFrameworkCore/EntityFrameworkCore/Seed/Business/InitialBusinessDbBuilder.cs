using SplitPackage.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.EntityFrameworkCore.Seed.Business
{
    public class InitialBusinessDbBuilder
    {
        private readonly SplitPackageDbContext _context;

        public InitialBusinessDbBuilder(SplitPackageDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultProductClassCreator(_context).Create();
            new DefaultProductCreator(_context, 1).Create();
            new DefaultProductProductClassCreator(_context).Create();
            new DefaultLogisticCreator(_context,1).Create();
            new DefaultLogisticLineCreator(_context,1).Create();
            new DefaultNumFreightCreator(_context,1).Create();
            new DefaultSplitRuleCreator(_context,1).Create();
            new DefaultWeightFreightCreator(_context, 1).Create();
            new DefaultSplitRuleProductClassCreator(_context).Create();
            _context.SaveChanges();
        }
    }
}
