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
        public virtual LogisticChannel LogisticChannelBy { get; set; }

        public long LogisticChannelId { get; set; }

        public bool IsActive { get; set; }

        public NumFreight()
        {
            IsActive = true;
        }

        /// <summary>
        /// 币种(暂无用)
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 单位(暂无用)
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 打包数量
        /// </summary>
        public int SplitNum { get; set; }

        /// <summary>
        /// 首件价格
        /// </summary>
        public double FirstPrice { get; set; }

        /// <summary>
        /// 续件价格
        /// </summary>
        public double CarryOnPrice { get; set; }
    }
}
