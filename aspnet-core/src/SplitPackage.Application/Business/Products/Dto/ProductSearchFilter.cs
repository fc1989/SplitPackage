using Abp.Application.Services.Dto;
using Newtonsoft.Json;
using SplitPackage.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Business.Products.Dto
{
    public class ProductSearchFilter : PageSearchFilter<Product>
    {
        public string ProductName { get; set; }

        public string Sku { get; set; }

        public string Brand { get; set; }

        [JsonProperty("ptid")]
        public string PTId { get; set; }
    }
}
