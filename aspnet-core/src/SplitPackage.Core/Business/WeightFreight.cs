using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SplitPackage.Business
{
    [Table("WeightFreights")]
    public class WeightFreight : FullAuditedEntity<long>, IPassivable
    {
        public long LogisticLineId { get; set; }

        /// <summary>
        /// 物流线路
        /// </summary>
        public virtual LogisticLine LogisticLineBy { get; set; }

        /// <summary>
        /// 首重
        /// </summary>
        public double StartingWeight { get; set; }

        /// <summary>
        /// 首重价格
        /// </summary>
        public double StartingPrice { get; set; }

        /// <summary>
        /// 续重重量
        /// </summary>
        public double StepWeight { get; set; }

        /// <summary>
        /// 续重价格
        /// </summary>
        public double Price { get; set; }

        public bool IsActive { get; set; }

        public WeightFreight()
        {
            IsActive = true;
        }
    }
}
