using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SplitPackage.Business
{
    [Table("LogisticRelateds")]
    public class LogisticRelated : Entity<long>, IMayHaveTenant
    {
        public const int MaxRelatedNameLength = 100;

        public string RelatedName { get; set; }

        public int? TenantId { get; set; }

        public virtual ICollection<LogisticRelatedItem> Items { get; set; }
    }

    [Table("LogisticRelatedItems")]
    public class LogisticRelatedItem : Entity<long>
    {
        public long LogisticRelatedId { get; set; }

        public long LogisticId { get; set; }

        public virtual LogisticRelated LogisticRelatedBy { get; set; }

        public virtual Logistic LogisticBy { get; set; }
    }
}
