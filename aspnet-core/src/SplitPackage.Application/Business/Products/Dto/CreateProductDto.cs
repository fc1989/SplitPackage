using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SplitPackage.Business.Products.Dto
{
    [AutoMap(typeof(Product))]
    public class CreateProductDto: EntityDto<long>
    {
        [Required]
        [StringLength(Product.MaxProductNameLength)]
        public string ProductName { get; set; }

        [Required]
        [StringLength(Product.MaxSkuLength)]
        public string Sku { get; set; }

        [StringLength(Product.MaxBrandLength)]
        public string Brand { get; set; }

        public double Weight { get; set; }

        public double DeclarePrice { get; set; }

        public double DeclareTaxrate { get; set; }

        [JsonProperty("ptid")]
        [Required]
        [StringLength(Product.MaxPTIdLength)]
        public string PTId { get; set; }
    }
}