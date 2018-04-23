using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SplitPackage.Business
{
    [Table("Products")]
    public class Product : FullAuditedEntity<long>, IPassivable, IMayHaveTenant
    {
        public const int MaxProductNameLength = 200;
        public const int MaxAbbreNameLength = 100;
        public const int MaxProductNoLength = 50;
        public const int MaxSkuLength = 50;
        public const int MaxTaxNoLength = 20;
        public const int MaxBrandLength = 50;
        public const double DefaultWeightValue = 0;

        public bool IsActive { get; set; }

        public int? TenantId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品简称
        /// </summary>
        public string AbbreName { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductNo { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// 税号
        /// </summary>
        public string TaxNo { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public double Weight { get; set; }

        public ICollection<ProductProductClass> ProductClasses { get; set; }

        public Product()
        {
            this.IsActive = true;
        }
    }
}
