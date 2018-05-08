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
        public const int MaxSkuLength = 50;
        public const int MaxBrandLength = 50;
        public const double DefaultWeightValue = 0;
        public const double DefaultDeclarePriceValue = 0;
        public const double DefaultDeclareTaxrateValue = 0;
        public const int MaxPTIdLength = 50;

        public bool IsActive { get; set; }

        public int? TenantId { get; set; }

        public string PTId { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// 申报价格
        /// </summary>
        public double DeclarePrice { get; set; }

        /// <summary>
        /// 申报税率
        /// </summary>
        public double DeclareTaxrate { get; set; }

        public Product()
        {
            this.IsActive = true;
        }
    }
}
