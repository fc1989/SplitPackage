using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SplitPackage.Business.ProductSorts.Dto
{
    [AutoMapFrom(typeof(ProductClass))]
    public class ProductClassDto : EntityDto<long>, IPassivable
    {
        public string SortName { get; set; }

        [Required]
        [StringLength(ProductClass.MaxClassNameLength)]
        public string ClassName { get; set; }

        [Required]
        [StringLength(ProductClass.MaxPTIdLength)]
        [JsonProperty("ptid")]
        public string PTId { get; set; }

        public double PostTaxRate { get; set; }

        [JsonProperty("bcTaxRate")]
        public double BCTaxRate { get; set; }

        public bool IsActive { get; set; }
    }
}
