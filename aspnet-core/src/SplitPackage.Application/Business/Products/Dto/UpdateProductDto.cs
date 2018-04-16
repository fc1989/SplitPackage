using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SplitPackage.Business.Products.Dto
{
    [AutoMap(typeof(Product))]
    public class UpdateProductDto: EntityDto<long>, IPassivable
    {
        [Required]
        [StringLength(Product.MaxProductNameLength)]
        public string ProductName { get; set; }

        [StringLength(Product.MaxAbbreNameLength)]
        public string AbbreName { get; set; }

        [Required]
        [StringLength(Product.MaxProductNoLength)]
        public string ProductNo { get; set; }

        [Required]
        [StringLength(Product.MaxSkuLength)]
        public string Sku { get; set; }

        [StringLength(Product.MaxTaxNoLength)]
        public string TaxNo { get; set; }

        [StringLength(Product.MaxBrandLength)]
        public string Brand { get; set; }

        public double Weight { get; set; }

        public bool IsActive { get; set; }
    }
}
