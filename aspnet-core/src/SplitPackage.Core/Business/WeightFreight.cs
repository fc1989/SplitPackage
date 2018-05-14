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
        public long LogisticChannelId { get; set; }

        /// <summary>
        /// 物流线路
        /// </summary>
        public virtual LogisticChannel LogisticChannelBy { get; set; }

        public bool IsActive { get; set; }

        public WeightFreight()
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
        /// 首重/阶梯开始重量
        /// </summary>
        public double StartingWeight { get; set; }

        /// <summary>
        /// 阶梯结束重量
        /// </summary>
        public double EndWeight { get; set; }

        /// <summary>
        /// 起始费用
        /// </summary>
        public double StartingPrice { get; set; }

        /// <summary>
        /// 续重重量/计费单位重量
        /// </summary>
        public double StepWeight { get; set; }

        /// <summary>
        /// 阶梯成本价
        /// </summary>
        public double CostPrice { get; set; }

        /// <summary>
        /// 续重价格/阶梯销售价
        /// </summary>
        public double Price { get; set; }
    }
}
