using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SplitPackage.Business
{
    [Table("ProductClasses")]
    public class ProductClass : FullAuditedEntity<long>, IPassivable
    {
        public const int MaxClassNameLength = 50;
        public const int MaxPTIdLength = 50;

        /// <summary>
        /// 类别名
        /// </summary>
        public string ClassName { get; set; }

        public string PTId { get; set; }

        /// <summary>
        /// 行邮税率
        /// </summary>
        public double PostTaxRate { get; set; }

        /// <summary>
        /// BC行邮税率
        /// </summary>
        public double BCTaxRate { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<ProductProductClass> Products { get; set; }

        public virtual ICollection<SplitRuleProductClass> SplitRules { get; set; }

        public ProductClass()
        {
            IsActive = true;
        }
    }
}
