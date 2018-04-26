using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SplitPackage.Business
{
    [Table("SplitRules")]
    public class SplitRule : FullAuditedEntity<long>, IPassivable, IMayHaveTenant
    {
        public long LogisticLineId { get; set; }

        /// <summary>
        /// 物流线路
        /// </summary>
        public virtual LogisticLine LogisticLineBy { get; set; }

        /// <summary>
        /// 包裹最大数量
        /// </summary>
        public int MaxPackage { get; set; }

        /// <summary>
        /// 重量限制
        /// </summary>
        public double MaxWeight { get; set; }

        /// <summary>
        /// 税费限制
        /// </summary>
        public double MaxTax { get; set; }

        /// <summary>
        /// 价格限制
        /// </summary>
        public double MaxPrice { get; set; }

        public bool IsActive { get; set; }

        public int? TenantId { get; set; }

        public virtual ICollection<SplitRuleProductClass> ProductClasses { get; set; }

        public SplitRule()
        {
            IsActive = true;
        }
    }
}
