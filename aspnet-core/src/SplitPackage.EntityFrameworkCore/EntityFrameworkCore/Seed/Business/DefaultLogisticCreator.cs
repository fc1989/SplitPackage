using SplitPackage.Business;
using System.Collections.Generic;
using System.Linq;

namespace SplitPackage.EntityFrameworkCore.Seed.Business
{
    public class DefaultLogisticCreator
    {
        private readonly SplitPackageDbContext _context;

        public DefaultLogisticCreator(SplitPackageDbContext context)
        {
            this._context = context;
        }

        public void Create()
        {
            AddIfNotExists(new Logistic()
            {
                CorporationName = "AOLAU EXPRESS",
                CorporationUrl = "http://www.aol-au.com",
                LogisticFlag = "AOLAU EXPRESS"
            });
        }

        private void AddIfNotExists(Logistic l)
        {
            if (_context.Logistics.Any(s => s.LogisticFlag == l.LogisticFlag))
            {
                return;
            }

            _context.Logistics.Add(l);
            _context.SaveChanges();

            var ll = new LogisticLine()
            {
                LineName = "通达速递奶粉专线",
                LineCode = "通达速递奶粉专线",
                IsActive = true,
                LogisticId = l.Id
            };
            _context.LogisticLines.Add(ll);
            _context.SaveChanges();

            _context.WeightFreights.Add(new WeightFreight()
            {
                LogisticLineId = ll.Id,
                StartingWeight = 1000,
                StartingPrice = 5,
                StepWeight = 100,
                Price = 0.5
            });
            var sr = new SplitRule()
            {
                MaxPackage = 3,
                MaxWeight = 40000,
                MaxTax = 10000,
                MaxPrice = 10000,
                LogisticLineId = ll.Id
            };
            _context.SplitRules.Add(sr);
            _context.SaveChanges();

            _context.SplitRuleProductClass.AddRange(new List<SplitRuleProductClass>()
            {
                new SplitRuleProductClass()
                {
                    PTId = "1010703",
                    SplitRuleId = sr.Id,
                    MinNum = 3,
                    MaxNum = 3
                },
                new SplitRuleProductClass()
                {
                    PTId = "1010704",
                    SplitRuleId = sr.Id,
                    MinNum = 3,
                    MaxNum = 3
                },
                new SplitRuleProductClass()
                {
                    PTId = "1010705",
                    SplitRuleId = sr.Id,
                    MinNum = 3,
                    MaxNum = 3
                },
                new SplitRuleProductClass()
                {
                    PTId = "1010706",
                    SplitRuleId = sr.Id,
                    MinNum = 3,
                    MaxNum = 3
                }
            });
            _context.SaveChanges();
        }
    }
}
