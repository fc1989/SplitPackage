using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SplitPackage.Business
{
    [Table("LogisticLines")]
    public class LogisticLine : FullAuditedEntity<long>, IPassivable
    {
        public const int MaxLineNameLength = 50;
        public const int MaxLineCodeLength = 50;

        /// <summary>
        /// 线路名称
        /// </summary>
        public string LineName { get; set; }

        /// <summary>
        /// 线路代码
        /// </summary>
        public string LineCode { get; set; }

        public bool IsActive { get; set; }

        public long LogisticId { get; set; }

        /// <summary>
        /// 物流企业
        /// </summary>
        public virtual Logistic LogisticBy { get; set; }

        public virtual ICollection<SplitRule> SplitRules { get; set; }

        public virtual ICollection<WeightFreight> WeightFreights { get; set; }

        public virtual ICollection<NumFreight> NumFreights { get; set; }

        public LogisticLine()
        {
            IsActive = true;
        }
    }
}
