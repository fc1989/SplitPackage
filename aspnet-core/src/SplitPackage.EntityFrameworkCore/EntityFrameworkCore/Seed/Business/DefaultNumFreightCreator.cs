using SplitPackage.EntityFrameworkCore;
using SplitPackage.Business;

namespace SplitPackage.EntityFrameworkCore.Seed.Business
{
    public class DefaultNumFreightCreator
    {
        private readonly SplitPackageDbContext _context;
        private readonly long _logisticLineId;

        public DefaultNumFreightCreator(SplitPackageDbContext context, long logisticLineId)
        {
            this._context = context;
            this._logisticLineId = logisticLineId;
        }

        public void Create()
        {
            var nf =new NumFreight()
            {
                LogisticLineId = this._logisticLineId,
                ProductNum = 10,
                PackagePrice = 20,
            };
            _context.NumFreights.Add(nf);
            _context.SaveChanges();
        }
    }
}
