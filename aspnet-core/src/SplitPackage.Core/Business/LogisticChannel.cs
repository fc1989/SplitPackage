using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SplitPackage.Business
{
    [Table("LogisticChannels")]
    public class LogisticChannel : FullAuditedEntity<long>, IPassivable,IMayHaveTenant
    {
        public const int MaxChannelNameLength = 50;
        public const int MaxAliasNameLength = 50;

        /// <summary>
        /// 渠道名称
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// 渠道别名
        /// </summary>
        public string AliasName { get; set; }

        /// <summary>
        /// 渠道类型
        /// </summary>
        public ChannelType Type { get; set; }

        /// <summary>
        /// 收费方式
        /// </summary>
        public ChargeWay Way { get; set; }

        public bool IsActive { get; set; }

        public long LogisticId { get; set; }

        /// <summary>
        /// 物流企业
        /// </summary>
        public virtual Logistic LogisticBy { get; set; }

        public virtual ICollection<SplitRule> SplitRules { get; set; }

        public virtual ICollection<WeightFreight> WeightFreights { get; set; }

        public virtual ICollection<NumFreight> NumFreights { get; set; }

        public int? TenantId { get; set; }

        public LogisticChannel()
        {
            IsActive = true;
        }
    }

    public enum ChannelType
    {
        BC = 0,
        CC = 1
    }

    public enum ChargeWay
    {
        ChargeByWeight = 0,
        ChargeByNum = 1
    }
}
