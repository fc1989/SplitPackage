using Abp.AutoMapper;
using SplitPackage.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Split.Dto
{
    public class ProductSortDto
    {
        public string SortName { get; set; }

        public IList<ProductClassDto> Items { get; set; }
    }

    public class ProductClassDto
    {
        public string ClassName { get; set; }

        public string PTId { get; set; }
    }
}
