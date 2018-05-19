using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SplitPackage.Business.ProductSorts.Dto
{
    [AutoMap(typeof(ProductClass))]
    public class CreateProductClassDto : EntityDto<long>
    {
        public long ProductSortId { get; set; }

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
    }
}