using Microsoft.EntityFrameworkCore;
using SplitPackage.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SplitPackage.EntityFrameworkCore.Seed.Business
{
    public class DefaultLogisticRelatedCreator
    {
        private readonly SplitPackageDbContext _context;

        public DefaultLogisticRelatedCreator(SplitPackageDbContext context)
        {
            this._context = context;
        }

        public void Create()
        {
            if (this._context.LogisticRelated.Any())
            {
                return;
            }
            var relatedItems = this._context.Logistics.AsQueryable().Where(o => new string[] { "EWE Express 标准线","EWE Express 经济线"}.Contains(o.LogisticCode));
            var related = new LogisticRelated()
            {
                RelatedName = "EWE Express"
            };
            related.Items = relatedItems.Select(o=> new LogisticRelatedItem() {
                LogisticId = o.Id,
                LogisticRelatedBy = related
            }).ToList();
            this._context.LogisticRelated.Add(related);
            this._context.SaveChanges();
        }
    }
}
