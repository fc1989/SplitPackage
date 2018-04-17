using SplitPackage.EntityFrameworkCore;
using SplitPackage.Business;

namespace SplitPackage.EntityFrameworkCore.Seed.Business
{
    public class DefaultWeightFreightCreator
    {
        private readonly SplitPackageDbContext _context;
        private readonly long _logisticLineId;

        public DefaultWeightFreightCreator(SplitPackageDbContext context, long logisticLineId)
        {
            this._context = context;
            this._logisticLineId = logisticLineId;
        }

        public void Create()
        {
            var nf = new WeightFreight()
            {
                LogisticLineId = this._logisticLineId,
                StartingWeight = 10,
                StartingPrice = 5,
                StepWeight = 10,
                Price = 10
            };
            _context.WeightFreights.Add(nf);
            _context.SaveChanges();
        }
    }
}
