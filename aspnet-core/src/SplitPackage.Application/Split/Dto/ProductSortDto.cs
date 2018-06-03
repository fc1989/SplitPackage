using Abp.AutoMapper;
using Newtonsoft.Json;
using SplitPackage.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Split.Dto
{
    public class ProductSortSimpleDto1
    {
        public string SortName { get; set; }

        public IList<ProductClassSimpleDto> Items { get; set; }
    }

    public class ProductClassSimpleDto
    {
        public string ClassName { get; set; }

        [JsonProperty("ptid")]
        public string PTId { get; set; }
    }
}
