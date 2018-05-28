using Abp.Application.Services.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.ProductSorts.Dto
{
    public class ProductSortSearchFilter : PagedResultRequestDto, ISortedResultRequest
    {
        public string SortName { get; set; }

        public string ClassName { get; set; }

        [JsonProperty("ptid")]
        public string PTId { get; set; }

        public string Sorting { get; set; }
    }
}
