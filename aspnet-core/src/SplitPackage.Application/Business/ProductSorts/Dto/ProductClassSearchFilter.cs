using SplitPackage.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.ProductSorts.Dto
{
    public class ProductClassSearchFilter : PageSearchFilter<ProductClass>
    {
        public long ProductSortId { get; set; }
    }
}
