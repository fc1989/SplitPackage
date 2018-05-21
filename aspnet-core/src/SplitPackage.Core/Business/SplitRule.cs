using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SplitPackage.Business
{
    [Table("SplitRules")]
    public class SplitRule : Entity<long>, IPassivable, IHasCreationTime
    {
        public const int MaxRuleNameLength = 50;

        public long LogisticChannelId { get; set; }

        /// <summary>
        /// 物流渠道
        /// </summary>
        public virtual LogisticChannel LogisticChannelBy { get; set; }

        /// <summary>
        /// 规则名
        /// </summary>
        public string RuleName { get; set; }

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
        /// 价值限制
        /// </summary>
        public double MaxPrice { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<SplitRuleProductClass> ProductClasses { get; set; }

        public DateTime CreationTime { get; set; }

        public SplitRule()
        {
            IsActive = true;
        }
    }
}
