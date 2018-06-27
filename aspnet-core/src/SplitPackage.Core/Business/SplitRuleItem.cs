using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SplitPackage.Business
{
    [Table("SplitRule_ProductClass")]
    public class SplitRuleItem: Entity<long>, IHasCreationTime, IMayHaveTenant
    {
        public string StintMark { get; set; }

        public long SplitRuleId { get; set; }

        public virtual SplitRule SplitRuleBy { get; set; }

        public int MinNum { get; set; }

        public int MaxNum { get; set; }

        public DateTime CreationTime { get; set; }

        public int? TenantId { get; set; }

        public RuleItemStintType Type { get; set; }
    }

    public enum RuleItemStintType
    {
        PTId = 0,
        Sku = 1
    }
}
