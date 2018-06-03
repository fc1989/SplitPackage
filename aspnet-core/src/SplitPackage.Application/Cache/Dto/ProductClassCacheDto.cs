using Abp.AutoMapper;
using SplitPackage.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Cache.Dto
{
    public class ProductClassCacheDto
    {
        public long ProductClassId { get; set; }

        public long ProductSortId { get; set; }

        public string SortName { get; set; }

        public string ClassName { get; set; }

        public string PTId { get; set; }

        public double PostTaxRate { get; set; }

        public double BCTaxRate { get; set; }

        public bool IsActive { get; set; }
    }
}
