using Microsoft.EntityFrameworkCore;
using SplitPackage.EntityFrameworkCore;
using System.Linq;
using SplitPackage.Business;

namespace SplitPackage.EntityFrameworkCore.Seed.Business
{
    public class DefaultLogisticLineCreator
    {
        private readonly SplitPackageDbContext _context;
        private readonly long _logisticId;

        public DefaultLogisticLineCreator(SplitPackageDbContext context, long logisticId)
        {
            this._context = context;
            this._logisticId = logisticId;
        }

        public void Create()
        {
            AddIfNotExists(new LogisticLine()
            {
                LineName = "物流线路1",
                LineCode = "wl01",
                LogisticId = this._logisticId
            });
        }

        private void AddIfNotExists(LogisticLine l)
        {
            if (_context.LogisticLines.IgnoreQueryFilters().Any(s => s.LogisticId == l.LogisticId && s.LineCode == l.LineCode))
            {
                return;
            }

            _context.LogisticLines.Add(l);
            _context.SaveChanges();
        }
    }
}
