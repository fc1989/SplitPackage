using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SplitPackage.Business.Products.Dto
{
    public class ProductDto : EntityDto<long>, IPassivable
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

        public bool IsActive { get; set; }

        public double DeclarePrice { get; set; }

        public double DeclareTaxrate { get; set; }

        [JsonProperty("ptid")]
        [Required]
        [StringLength(Product.MaxPTIdLength)]
        public string PTId { get; set; }
    }
}
