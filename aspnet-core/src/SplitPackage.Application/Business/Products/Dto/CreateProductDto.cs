using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SplitPackage.Business.Products.Dto
{
    [AutoMap(typeof(Product))]
    public class CreateProductDto: EntityDto<long>
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
    }
}