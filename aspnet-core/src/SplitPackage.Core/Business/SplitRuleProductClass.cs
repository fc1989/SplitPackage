using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SplitPackage.Business
{
    [Table("SplitRule_ProductClass")]
    public class SplitRuleProductClass: Entity<long>
    {
        public string PTId { get; set; }

        public long SplitRuleId { get; set; }

        public virtual SplitRule SplitRuleBy { get; set; }

        public int MinNum { get; set; }

        public int MaxNum { get; set; }
    }
}
