using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SplitPackage.Business
{
    [Table("NumFreights")]
    public class NumFreight : FullAuditedEntity<long>, IPassivable
    {
        /// <summary>
        /// 物流线路
        /// </summary>
        public virtual LogisticLine LogisticLineBy { get; set; }

        public long LogisticLineId { get; set; }

        /// <summary>
        /// 包裹商品数
        /// </summary>
        public int ProductNum { get; set; }

        /// <summary>
        /// 包裹价格
        /// </summary>
        public double PackagePrice { get; set; }

        public bool IsActive { get; set; }

        public NumFreight()
        {
            IsActive = true;
        }
    }
}
